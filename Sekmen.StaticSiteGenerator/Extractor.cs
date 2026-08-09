namespace Sekmen.StaticSiteGenerator;

public static class Extractor
{
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
}
