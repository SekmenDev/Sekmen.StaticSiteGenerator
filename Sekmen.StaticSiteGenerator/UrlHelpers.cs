namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Provides utility methods for extracting, queuing, resolving, and rewriting URLs during static site generation.
/// </summary>
public static class UrlHelpers
{
    /// <summary>
    /// Extracts page location (<c>&lt;loc&gt;</c>) elements from an XML sitemap document into a processing queue.
    /// </summary>
    /// <param name="sitemap">The parsed XML sitemap document.</param>
    /// <param name="ns">The XML namespace of the sitemap elements.</param>
    /// <returns>A <see cref="Queue{T}"/> containing all extracted sitemap URL strings.</returns>
    public static Queue<string> EnqueueSitemapUrls(XDocument sitemap, XNamespace ns)
    {
        Queue<string> urls = new();
        foreach (XElement loc in sitemap.Descendants(ns + "loc"))
            urls.Enqueue(loc.Value);
        return urls;
    }

    /// <summary>
    /// Resolves additional user-specified paths against the base source URL and adds them to the processing queue.
    /// </summary>
    /// <param name="urls">The target URL queue to add resolved URLs into.</param>
    /// <param name="additionalPaths">A collection of relative or absolute path strings to enqueue.</param>
    /// <param name="sourceUrl">The base source URL of the site.</param>
    public static void EnqueueAdditionalUrls(Queue<string> urls, IEnumerable<string> additionalPaths, string sourceUrl)
    {
        foreach (string path in additionalPaths)
            urls.Enqueue(new Uri(new Uri(sourceUrl), path).ToString());
    }

    /// <summary>
    /// Scans a collection of HTML hyperlink nodes for internal links and enqueues newly discovered, unvisited page URLs.
    /// </summary>
    /// <param name="links">The collection of hyperlink (<c>&lt;a href="..."&gt;</c>) HTML nodes.</param>
    /// <param name="pageUrl">The full URL of the current page containing the links.</param>
    /// <param name="visited">A set tracking already visited or queued page URLs.</param>
    /// <param name="urls">The queue to receive newly discovered internal page URLs.</param>
    public static void EnqueueInternalLinks(HtmlNodeCollection links, string pageUrl, HashSet<string> visited, Queue<string> urls)
    {
        Uri pageUri = new(pageUrl);
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (HtmlNode link in links)
        {
            string href = link.GetAttributeValue("href", string.Empty);
            if (!UrlValidation.IsInternalLink(href)) continue;

            Uri resolvedUri = new(pageUri, href);
            if (resolvedUri.Host == pageUri.Host && !visited.Contains(resolvedUri.ToString()))
                urls.Enqueue(resolvedUri.ToString());
        }
    }

    /// <summary>
    /// Replaces occurrences of the source domain, relative paths, and custom string patterns in raw HTML with the configured target URL.
    /// </summary>
    /// <param name="html">The raw HTML string to process.</param>
    /// <param name="sourceUrl">The original site base URL to replace.</param>
    /// <param name="command">The export command configuration containing target URL and string replacements.</param>
    /// <returns>The modified HTML string with updated URL references.</returns>
    public static string UpdateHtmlUrls(string html, string sourceUrl, ExportCommand command)
    {
        string targetUrlWithSlash = command.TargetUrl.EndsWith('/') ? command.TargetUrl : command.TargetUrl + "/";
        string targetUrlWithoutSlash = command.TargetUrl.TrimEnd('/');

        Uri sourceUri = new(sourceUrl);
        string hostAndPathWithSlash = sourceUri.Authority + sourceUri.AbsolutePath;
        if (!hostAndPathWithSlash.EndsWith('/')) hostAndPathWithSlash += "/";
        string hostAndPathWithoutSlash = hostAndPathWithSlash.TrimEnd('/');

        string updatedHtml = html
            .Replace($"https://{hostAndPathWithSlash}", targetUrlWithSlash)
            .Replace($"http://{hostAndPathWithSlash}", targetUrlWithSlash)
            .Replace($"//{hostAndPathWithSlash}", targetUrlWithSlash)
            .Replace($"https://{hostAndPathWithoutSlash}", targetUrlWithoutSlash)
            .Replace($"http://{hostAndPathWithoutSlash}", targetUrlWithoutSlash)
            .Replace($"//{hostAndPathWithoutSlash}", targetUrlWithoutSlash);

        updatedHtml = Regex.Replace(
            updatedHtml,
            """(?<=\b[a-zA-Z0-9\-_:]+\s*=\s*|url\(\s*)(["']?)/(?!/)""",
            match => $"{match.Groups[1].Value}{targetUrlWithSlash}"
        );

        return command.StringReplacements.Aggregate(updatedHtml, (current, replacement) => 
            current.Replace(replacement.OldValue, replacement.NewValue));
    }
}