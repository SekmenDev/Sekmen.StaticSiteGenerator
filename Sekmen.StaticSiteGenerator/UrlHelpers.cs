namespace Sekmen.StaticSiteGenerator;

public static class UrlHelpers
{
    public static Queue<string> EnqueueSitemapUrls(XDocument sitemap, XNamespace ns)
    {
        Queue<string> urls = new();
        foreach (XElement loc in sitemap.Descendants(ns + "loc"))
            urls.Enqueue(loc.Value);
        return urls;
    }

    public static void EnqueueAdditionalUrls(Queue<string> urls, IEnumerable<string> additionalPaths, string sourceUrl)
    {
        foreach (string path in additionalPaths)
            urls.Enqueue(new Uri(new Uri(sourceUrl), path).ToString());
    }

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

    public static string UpdateHtmlUrls(string html, string sourceUrl, ExportCommand command)
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
}