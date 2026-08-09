// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Sekmen.StaticSiteGenerator;

public static class Functions
{
    private static class UrlValidation
    {
        private static readonly string[] ResourceExtensions = [".pdf", ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".svg", ".ico"];
        private static readonly string[] ExternalSchemes = ["//", "http://", "https://", "www."];
        private static readonly string[] ExcludedPrefixes = ["#", "mailto:", "tel:"];

        public static bool IsResourceFile(string urlPath) =>
            ResourceExtensions.Any(ext => urlPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

        public static bool IsInternalLink(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return false;
            if (ExcludedPrefixes.Any(href.StartsWith)) return false;
            if (ExternalSchemes.Any(href.StartsWith)) return false;
            return href.StartsWith('/');
        }

        public static bool IsValidResourceUrl(string url, Uri baseUri) =>
            !string.IsNullOrWhiteSpace(url) &&
            !url.StartsWith("//") &&
            (url.StartsWith('/') || url.StartsWith(baseUri.AbsoluteUri)) &&
            !url.Equals(baseUri.AbsoluteUri);
    }

    private static class Logger
    {
        public static void Info(string message) => Console.WriteLine(message);
        public static void Error(string message, Exception? ex = null) =>
            Console.WriteLine(ex != null ? $"{message}: {ex.Message}" : message);
    }

    private static string NormalizeSourceUrl(string siteUrl) =>
        siteUrl.Contains("://") 
            ? (siteUrl.EndsWith('/') ? siteUrl : siteUrl + "/")
            : $"https://{siteUrl}/";

    public static async Task ExportWebsite(HttpClient client, ExportCommand command)
    {
        string sourceUrl = NormalizeSourceUrl(command.SiteUrl);
        Directory.CreateDirectory(command.OutputFolder);
        
        string sitemapXml = await client.GetStringAsync(sourceUrl + "sitemap.xml");
        XDocument sitemap = XDocument.Parse(sitemapXml);
        XNamespace ns = sitemap.Root!.GetDefaultNamespace();

        HashSet<string> visited = [];
        Queue<string> urls = EnqueueSitemapUrls(sitemap, ns);
        EnqueueAdditionalUrls(urls, command.AdditionalUrls, sourceUrl);

        while (urls.Count > 0)
        {
            string pageUrl = urls.Dequeue();
            if (!visited.Add(pageUrl)) continue;

            Logger.Info($"Processing: {pageUrl}");
            HtmlDocument? htmlDoc = await ProcessPage(client, pageUrl, sourceUrl, command);

            HtmlNodeCollection? links = htmlDoc?.DocumentNode.SelectNodes("//a[@href]");
            if (links != null)
                EnqueueInternalLinks(links, pageUrl, visited, urls);
        }
    }

    private static Queue<string> EnqueueSitemapUrls(XDocument sitemap, XNamespace ns)
    {
        Queue<string> urls = new();
        foreach (XElement loc in sitemap.Descendants(ns + "loc"))
            urls.Enqueue(loc.Value);
        return urls;
    }

    private static void EnqueueAdditionalUrls(Queue<string> urls, IEnumerable<string> additionalPaths, string sourceUrl)
    {
        foreach (string path in additionalPaths)
            urls.Enqueue(new Uri(new Uri(sourceUrl), path).ToString());
    }

    private static void EnqueueInternalLinks(HtmlNodeCollection links, string pageUrl, HashSet<string> visited, Queue<string> urls)
    {
        Uri pageUri = new(pageUrl);
        foreach (HtmlNode link in links)
        {
            string href = link.GetAttributeValue("href", string.Empty);
            if (!UrlValidation.IsInternalLink(href)) continue;

            Uri resolvedUri = new(pageUri, href);
            if (resolvedUri.Host == pageUri.Host && !visited.Contains(resolvedUri.ToString()))
                urls.Enqueue(resolvedUri.ToString());
        }
    }

    private static async Task<HtmlDocument?> ProcessPage(HttpClient client, string pageUrl, string sourceUrl, ExportCommand command)
    {
        try
        {
            Uri uri = new(pageUrl);
            string path = uri.AbsolutePath.ToLower();

            if (UrlValidation.IsResourceFile(path))
            {
                await DownloadResourceFile(client, pageUrl, uri, command.OutputFolder);
                return null;
            }

            string html = await client.GetStringAsync(pageUrl);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            string pagePath = BuildPagePath(uri, command);
            string updatedHtml = UpdateHtmlUrls(html, sourceUrl, command);
            await SaveHtmlFile(pagePath, updatedHtml);

            HashSet<string> resourceUrls = ExtractResourceUrls(htmlDoc, uri);
            foreach (string resourceUrl in resourceUrls)
                await DownloadResource(client, uri, resourceUrl, command.OutputFolder);

            return htmlDoc;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error processing {pageUrl}", ex);
        }

        return null;
    }

    private static async Task DownloadResourceFile(HttpClient client, string pageUrl, Uri uri, string outputFolder)
    {
        try
        {
            byte[] fileContent = await client.GetByteArrayAsync(pageUrl);
            string filePath = Path.Combine(outputFolder, uri.AbsolutePath.TrimStart('/'));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllBytesAsync(filePath, fileContent);
            Logger.Info($"File downloaded: {filePath}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to download resource {pageUrl}", ex);
        }
    }

    private static string BuildPagePath(Uri uri, ExportCommand command)
    {
        string pagePath = Path.Combine(command.OutputFolder, uri.AbsolutePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        pagePath = command.StringReplacements.Aggregate(pagePath, (current, replacement) => 
            current.Replace(replacement.OldValue, replacement.NewValue));

        if (!Path.HasExtension(uri.AbsolutePath))
            pagePath = Path.Combine(pagePath, "index.html");

        Directory.CreateDirectory(Path.GetDirectoryName(pagePath)!);
        return pagePath;
    }

    private static string UpdateHtmlUrls(string html, string sourceUrl, ExportCommand command)
    {
        string updatedHtml = html
            .Replace("\"" + sourceUrl.Replace("https:", "").Replace("http:", ""), "\"" + command.TargetUrl)
            .Replace("'" + sourceUrl.Replace("https:", "").Replace("http:", ""), "'" + command.TargetUrl)
            .Replace("\"/", "\"" + command.TargetUrl)
            .Replace("'/", "'" + command.TargetUrl)
            .Replace(sourceUrl, command.TargetUrl);

        return command.StringReplacements.Aggregate(updatedHtml, (current, replacement) => 
            current.Replace(replacement.OldValue, replacement.NewValue));
    }

    private static async Task SaveHtmlFile(string pagePath, string content)
    {
        await File.WriteAllTextAsync(pagePath, content);
        Logger.Info($"Page saved: {pagePath}");
    }

    private static HashSet<string> ExtractResourceUrls(HtmlDocument doc, Uri baseUri)
    {
        HashSet<string> resources = [];
        
        ExtractFromNodeAttribute(doc, "//link", "href", resources, baseUri);
        ExtractFromNodeAttribute(doc, "//script[@src]", "src", resources, baseUri);
        ExtractFromNodeAttribute(doc, "//img[@src]", "src", resources, baseUri);
        ExtractFromInlineStyles(doc, resources, baseUri);
        ExtractFromStyleTags(doc, resources, baseUri);

        return resources;
    }

    private static void ExtractFromNodeAttribute(HtmlDocument doc, string xPath, string attributeName, HashSet<string> resources, Uri baseUri)
    {
        HtmlNodeCollection? nodes = doc.DocumentNode.SelectNodes(xPath);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (nodes == null) return;

        foreach (HtmlNode node in nodes)
        {
            string url = node.GetAttributeValue(attributeName, string.Empty);
            if (UrlValidation.IsValidResourceUrl(url, baseUri))
                resources.Add(url);
        }
    }

    private static void ExtractFromInlineStyles(HtmlDocument doc, HashSet<string> resources, Uri baseUri)
    {
        HtmlNodeCollection? nodes = doc.DocumentNode.SelectNodes("//*[@style]");
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (nodes == null) return;

        foreach (HtmlNode node in nodes)
        {
            string style = node.GetAttributeValue("style", string.Empty);
            if (!string.IsNullOrEmpty(style))
                ExtractUrlsFromCss(style, resources, baseUri);
        }
    }

    private static void ExtractFromStyleTags(HtmlDocument doc, HashSet<string> resources, Uri baseUri)
    {
        HtmlNodeCollection? nodes = doc.DocumentNode.SelectNodes("//style");
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (nodes == null) return;

        foreach (HtmlNode node in nodes)
        {
            string cssContent = node.InnerText;
            if (!string.IsNullOrEmpty(cssContent))
                ExtractUrlsFromCss(cssContent, resources, baseUri);
        }
    }

    private static void ExtractUrlsFromCss(string cssContent, HashSet<string> resources, Uri baseUri)
    {
        MatchCollection matches = Regex.Matches(cssContent, """url\(['"]?(?<url>[^'"\)]+)['"]?\)""");
        foreach (Match match in matches)
        {
            string url = match.Groups["url"].Value;
            if (UrlValidation.IsValidResourceUrl(url, baseUri))
                resources.Add(url);
        }
    }

    private static async Task DownloadResource(HttpClient client, Uri pageUri, string resourceUrl, string outputFolder)
    {
        try
        {
            Uri resourceUri = new(pageUri, resourceUrl);
            string resourcePath = Path.Combine(outputFolder, resourceUri.AbsolutePath.TrimStart('/'));
            Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

            if (File.Exists(resourcePath))
                return;

            byte[] data = await client.GetByteArrayAsync(resourceUri);
            await File.WriteAllBytesAsync(resourcePath, data);
            Logger.Info($"Downloaded: {resourceUri}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed: {resourceUrl}", ex);
        }
    }
}
