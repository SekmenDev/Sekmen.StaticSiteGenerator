namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Provides methods for extracting linked and embedded static resource URLs (CSS, JavaScript, images, and inline styles) from HTML documents.
/// </summary>
public static class Extractor
{
    /// <summary>
    /// Extracts all valid static resource URLs referenced within an HTML document, including stylesheets, scripts, images, and CSS rules.
    /// </summary>
    /// <param name="doc">The parsed <see cref="HtmlDocument"/> to scan for resource URLs.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> of the HTML document used to resolve and validate relative resource URLs.</param>
    /// <returns>A <see cref="HashSet{T}"/> containing unique, validated resource URL strings.</returns>
    public static HashSet<string> ExtractResourceUrls(HtmlDocument doc, Uri baseUri)
    {
        HashSet<string> resources = [];
        
        ExtractFromNodeAttribute(doc, "//link", "href", resources, baseUri);
        ExtractFromNodeAttribute(doc, "//script[@src]", "src", resources, baseUri);
        ExtractFromNodeAttribute(doc, "//img[@src]", "src", resources, baseUri);
        ExtractFromInlineStyles(doc, resources, baseUri);
        ExtractFromStyleTags(doc, resources, baseUri);

        return resources;
    }

    /// <summary>
    /// Selects HTML nodes matching an XPath expression and extracts valid resource URLs from a specified attribute.
    /// </summary>
    /// <param name="doc">The parsed <see cref="HtmlDocument"/>.</param>
    /// <param name="xPath">The XPath query expression to locate target elements.</param>
    /// <param name="attributeName">The name of the attribute containing the target URL (e.g., "href" or "src").</param>
    /// <param name="resources">The collection into which extracted resource URLs are added.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> used for URL validation.</param>
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

    /// <summary>
    /// Extracts resource URLs referenced inside element inline <c>style</c> attributes across the HTML document.
    /// </summary>
    /// <param name="doc">The parsed <see cref="HtmlDocument"/>.</param>
    /// <param name="resources">The collection into which extracted resource URLs are added.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> used for URL validation.</param>
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

    /// <summary>
    /// Extracts resource URLs referenced within embedded HTML <c>&lt;style&gt;</c> tags across the document.
    /// </summary>
    /// <param name="doc">The parsed <see cref="HtmlDocument"/>.</param>
    /// <param name="resources">The collection into which extracted resource URLs are added.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> used for URL validation.</param>
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

    /// <summary>
    /// Parses CSS content using regular expressions to find and extract <c>url(...)</c> references.
    /// </summary>
    /// <param name="cssContent">The CSS snippet or stylesheet text to parse.</param>
    /// <param name="resources">The collection into which extracted resource URLs are added.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> used for URL validation.</param>
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
}
