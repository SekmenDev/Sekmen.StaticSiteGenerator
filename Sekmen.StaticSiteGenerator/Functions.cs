// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Sekmen.StaticSiteGenerator;

public static class Functions
{
    public static async Task ExportWebsite(HttpClient client, ExportCommand command)
    {
        string sourceUrl = command.SiteUrl.Contains("://") 
            ? (command.SiteUrl.EndsWith('/') ? command.SiteUrl : command.SiteUrl + "/")
            : $"https://{command.SiteUrl}/";
        Directory.CreateDirectory(command.OutputFolder);
        string sitemapXml = await client.GetStringAsync(sourceUrl + "sitemap.xml");
        XDocument sitemap = XDocument.Parse(sitemapXml);
        XNamespace ns = sitemap.Root!.GetDefaultNamespace();

        HashSet<string> visited = [];
        Queue<string> urls = new();

        // Load sitemap URLs
        foreach (XElement loc in sitemap.Descendants(ns + "loc")) 
            urls.Enqueue(loc.Value);

        // Add additional manual URLs
        foreach (string path in command.AdditionalUrls) 
            urls.Enqueue(new Uri(new Uri(sourceUrl), path).ToString());

        while (urls.Count > 0)
        {
            string pageUrl = urls.Dequeue();
            if (!visited.Add(pageUrl)) continue;

            // Extract internal links and enqueue them
            Console.WriteLine($"Processing: {pageUrl}");
            HtmlDocument? htmlDoc = await ProcessUrls(client, pageUrl, sourceUrl, command);
            HtmlNodeCollection? links = htmlDoc?.DocumentNode.SelectNodes("//a[@href]");
            if (links == null) continue;

            foreach (HtmlNode link in links)
            {
                // Extract href attribute
                string href = link.GetAttributeValue("href", string.Empty);
                if (string.IsNullOrWhiteSpace(href)) continue;

                // Normalize and filter URLs
                if (href.StartsWith('#') || href.StartsWith("mailto:") || href.StartsWith("tel:")) 
                    continue;
                if (href.StartsWith("//") || href.StartsWith("http://") || href.StartsWith("https://") || href.StartsWith("www.")) 
                    continue;
                if (!href.StartsWith('/')) 
                    continue;

                // Construct full URL and enqueue if not visited
                Uri uri = new(pageUrl);
                Uri newUri = new(uri, href);
                if (newUri.Host == uri.Host && !visited.Contains(newUri.ToString()))
                    urls.Enqueue(newUri.ToString());
            }
        }
    }

    /// <summary>
    /// 1. Fetch page content
    /// 2. Update URLs in HTML content
    /// 3. Extract and download resources
    /// 4. Save HTML content
    /// 5. Return HTML
    /// </summary>
    /// <param name="client">HttpClient</param>
    /// <param name="pageUrl">current page url as string</param>
    /// <param name="sourceUrl">source website url, domain</param>
    /// <param name="command">ExportCommand</param>
    /// <returns>HtmlDocument</returns>
    private static async Task<HtmlDocument?> ProcessUrls(HttpClient client, string pageUrl, string sourceUrl, ExportCommand command)
    {
        try
        {
            // Fetch page content
            string html = await client.GetStringAsync(pageUrl);
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            // Determine output path
            Uri uri = new(pageUrl);
            string pagePath = Path.Combine(command.OutputFolder, uri.AbsolutePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            pagePath = command.StringReplacements.Aggregate(pagePath, (current, replacements) => current.Replace(replacements.OldValue, replacements.NewValue));

            // Ensure directory exists and save HTML
            if (!Path.HasExtension(uri.AbsolutePath))
                pagePath = Path.Combine(pagePath, "index.html");
            if (!Directory.Exists(Path.GetDirectoryName(pagePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(pagePath)!);

            // Update URLs in HTML content
            // TODO: fix these replacements. unexpected replacement result: https://huseyinsekmenoglu.net//www.googletagmanager.com/gtag/js
            string updatedHtml = html
                .Replace("\"" + sourceUrl.Replace("https:", "").Replace("http:", ""), "\"" + command.TargetUrl)
                .Replace("'" + sourceUrl.Replace("https:", "").Replace("http:", ""), "'" + command.TargetUrl)
                .Replace("\"/", "\"" + command.TargetUrl)
                .Replace("'/", "'" + command.TargetUrl)
                .Replace(sourceUrl, command.TargetUrl);
            updatedHtml = command.StringReplacements
                .Aggregate(updatedHtml, (current, replacements) => 
                    current.Replace(replacements.OldValue, replacements.NewValue));
            await File.WriteAllTextAsync(pagePath, updatedHtml);
            Console.WriteLine($"Page saved: {pagePath}");

            if (pagePath.Contains(".pdf"))
                return null;

            // Extract and download resources
            HashSet<string> resourceUrls = ExtractResourceUrls(htmlDoc, uri);
            foreach (string resourceUrl in resourceUrls) 
                await DownloadResource(client, uri, resourceUrl, command.OutputFolder);

            return htmlDoc;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {pageUrl}: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Extracts URLs from HTML content
    /// 1. <link> tags
    /// 2. <script> tags
    /// 3. <img> tags
    /// 4. Inline styles
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="baseUri"></param>
    /// <returns>array of urls as string</returns>
    private static HashSet<string> ExtractResourceUrls(HtmlDocument doc, Uri baseUri)
    {
        HashSet<string> resources = [];

        // Extract from <link>, <script>, and <img> tags
        // Added detailed notifications for link tag processing (line 110 original reference)
        int linkIndex = 0;
        HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//link") ?? new HtmlNodeCollection(null!);
        foreach (HtmlNode link in linkNodes)
        {
            try
            {
                linkIndex++;
                string href = link.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrWhiteSpace(href) &&
                    !href.StartsWith("//") &&
                    (href.StartsWith('/') || href.StartsWith(baseUri.AbsoluteUri)) &&
                    !href.Equals(baseUri.AbsoluteUri))
                {
                    resources.Add(href);
                    Console.WriteLine($"[Link {linkIndex}] Added resource: {href}");
                }
                else
                {
                    Console.WriteLine($"[Link {linkIndex}] Skipped: '{href}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Link {linkIndex}] Error: {ex.Message}");
            }
        }

        // Extract from <script> tags
        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//script[@src]") ?? new HtmlNodeCollection(null!);
        foreach (HtmlNode script in nodes)
        {
            string src = script.GetAttributeValue("src", string.Empty);
            if (!string.IsNullOrWhiteSpace(src) &&
                !src.StartsWith("//") &&
                (src.StartsWith('/') || src.StartsWith(baseUri.AbsoluteUri)))
                resources.Add(src);
        }

        // Extract from <img> tags
        HtmlNodeCollection imgNodes = doc.DocumentNode.SelectNodes("//img[@src]") ?? new HtmlNodeCollection(null!);
        foreach (HtmlNode img in imgNodes)
        {
            string src = img.GetAttributeValue("src", string.Empty);
            if (!string.IsNullOrWhiteSpace(src) &&
                    !src.StartsWith("//") &&
                    (src.StartsWith('/') || src.StartsWith(baseUri.AbsoluteUri)))
                resources.Add(src);
        }


        // style="background-image: url(...)" inline styles
        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//*[@style]") ?? new HtmlNodeCollection(null!))
        {
            string style = node.GetAttributeValue("style", string.Empty);
            if (string.IsNullOrEmpty(style)) 
                continue;

            // Extract all urls from the style content using regex
            MatchCollection matches = Regex.Matches(style, """url\(['"]?(?<url>[^'"\)]+)['"]?\)""");
            foreach (Match match in matches)
            {
                string url = match.Groups["url"].Value;
                if (!string.IsNullOrWhiteSpace(url))
                    resources.Add(url);
            }
        }

        return resources;
    }

    /// <summary>
    /// Downloads a resource from the given URL and saves it to the specified output folder.
    /// If the resource already exists and has the same size, it is not downloaded again.
    /// 1. Determine full resource URL and local path
    /// 2. Check if the resource needs to be downloaded
    /// 3. Get remote file size
    /// 4. If file exists, compare sizes
    /// 5. Download the resource
    /// </summary>
    /// <param name="client">HttpClient</param>
    /// <param name="uri"></param>
    /// <param name="resourceUrl"></param>
    /// <param name="outputFolder"></param>
    private static async Task DownloadResource(HttpClient client, Uri uri, string resourceUrl, string outputFolder)
    {
        try
        {
            // Determine full resource URL and local path
            Uri resourceUri = new(uri, resourceUrl);
            string resourcePath = Path.Combine(outputFolder, resourceUri.AbsolutePath.TrimStart('/'));
            Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

            // Check if the resource needs to be downloaded
            using HttpRequestMessage headRequest = new(HttpMethod.Head, resourceUri);
            using HttpResponseMessage headResponse = await client.SendAsync(headRequest);
            headResponse.EnsureSuccessStatusCode();

            // Get remote file size
            long remoteSize = headResponse.Content.Headers.ContentLength ?? -1;
            bool shouldDownload = true;

            // If file exists, compare sizes
            if (File.Exists(resourcePath) && remoteSize != -1)
            {
                long localSize = new FileInfo(resourcePath).Length;
                shouldDownload = localSize != remoteSize;
            }

            if (shouldDownload)
            {
                byte[] data = await client.GetByteArrayAsync(resourceUri);
                await File.WriteAllBytesAsync(resourcePath, data);
                Console.WriteLine($"Downloaded: {resourceUri}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {resourceUrl} - {ex.Message}");
        }
    }
}
