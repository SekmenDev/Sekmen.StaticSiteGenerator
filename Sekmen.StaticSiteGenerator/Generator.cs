namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Orchestrates the process of crawling a website, downloading pages and assets, updating internal links, and generating a static output site.
/// </summary>
public static class Generator
{
    /// <summary>
    /// Exports a website to a static folder by crawling sitemap entries, additional URLs, and internal page links.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used to retrieve remote web pages and resources.</param>
    /// <param name="command">The <see cref="ExportCommand"/> options containing target URLs, output paths, and replacement configurations.</param>
    /// <returns>A task representing the complete asynchronous website export process.</returns>
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

    /// <summary>
    /// Processes an individual page URL by fetching its content, updating links, writing output files, and downloading referenced assets.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used for network requests.</param>
    /// <param name="pageUrl">The full absolute URL of the page being processed.</param>
    /// <param name="sourceUrl">The base source site URL.</param>
    /// <param name="command">The export command configuration.</param>
    /// <returns>A task returning the parsed <see cref="HtmlDocument"/> if processed as HTML, or <c>null</c> if it was a standalone resource file or failed.</returns>
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

    /// <summary>
    /// Constructs the destination file system path for saving a page HTML file based on its URI and string replacement settings.
    /// </summary>
    /// <param name="uri">The <see cref="Uri"/> of the page being saved.</param>
    /// <param name="command">The export command configuration specifying output folder and path replacements.</param>
    /// <returns>The resolved file system path where the HTML document should be stored.</returns>
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
    
    /// <summary>
    /// Writes the processed HTML content to disk asynchronously and logs the event.
    /// </summary>
    /// <param name="pagePath">The full target file path where the HTML should be written.</param>
    /// <param name="content">The HTML text content to save.</param>
    /// <returns>A task representing the file write operation.</returns>
    private static async Task SaveHtmlFile(string pagePath, string content)
    {
        await File.WriteAllTextAsync(pagePath, content);
        Logger.Info($"Page saved: {pagePath}");
    }
}
