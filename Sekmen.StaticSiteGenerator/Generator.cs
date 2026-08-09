namespace Sekmen.StaticSiteGenerator;

public static class Generator
{
    public static async Task ExportWebsite(HttpClient client, ExportCommand command)
    {
        string sourceUrl = UrlValidation.NormalizeSourceUrl(command.SiteUrl);
        Directory.CreateDirectory(command.OutputFolder);
        
        string sitemapXml = await client.GetStringAsync(sourceUrl + "sitemap.xml");
        XDocument sitemap = XDocument.Parse(sitemapXml);
        XNamespace ns = sitemap.Root!.GetDefaultNamespace();

        HashSet<string> visited = [];
        Queue<string> urls = UrlHelpers.EnqueueSitemapUrls(sitemap, ns);
        UrlHelpers.EnqueueAdditionalUrls(urls, command.AdditionalUrls, sourceUrl);

        while (urls.Count > 0)
        {
            string pageUrl = urls.Dequeue();
            if (!visited.Add(pageUrl)) continue;

            Logger.Info($"Processing: {pageUrl}");
            HtmlDocument? htmlDoc = await ProcessPage(client, pageUrl, sourceUrl, command);

            HtmlNodeCollection? links = htmlDoc?.DocumentNode.SelectNodes("//a[@href]");
            if (links != null)
                UrlHelpers.EnqueueInternalLinks(links, pageUrl, visited, urls);
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
                await Downloader.DownloadResourceFile(client, pageUrl, uri, command.OutputFolder);
                return null;
            }

            string html = await client.GetStringAsync(pageUrl);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            string pagePath = BuildPagePath(uri, command);
            string updatedHtml = UrlHelpers.UpdateHtmlUrls(html, sourceUrl, command);
            await SaveHtmlFile(pagePath, updatedHtml);

            HashSet<string> resourceUrls = Extractor.ExtractResourceUrls(htmlDoc, uri);
            foreach (string resourceUrl in resourceUrls)
                await Downloader.DownloadResource(client, uri, resourceUrl, command.OutputFolder);

            return htmlDoc;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error processing {pageUrl}", ex);
        }

        return null;
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
    
    private static async Task SaveHtmlFile(string pagePath, string content)
    {
        await File.WriteAllTextAsync(pagePath, content);
        Logger.Info($"Page saved: {pagePath}");
    }
}
