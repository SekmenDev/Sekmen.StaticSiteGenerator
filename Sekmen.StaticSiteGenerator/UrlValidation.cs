namespace Sekmen.StaticSiteGenerator;

public static class UrlValidation
{
    private static readonly string[] ResourceExtensions = [".pdf", ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".svg", ".ico"];
    private static readonly string[] ExternalSchemes = ["//", "http://", "https://", "www."];
    private static readonly string[] ExcludedPrefixes = ["#", "mailto:", "tel:"];

    public static bool IsResourceFile(string urlPath) =>
        ResourceExtensions.Any(ext => urlPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

    public static bool IsInternalLink(string href) =>
        !string.IsNullOrWhiteSpace(href) &&
        !ExcludedPrefixes.Any(href.StartsWith) &&
        !ExternalSchemes.Any(href.StartsWith) &&
        href.StartsWith('/');

    public static bool IsValidResourceUrl(string url, Uri baseUri) =>
        !string.IsNullOrWhiteSpace(url) &&
        !url.StartsWith("//") &&
        (url.StartsWith('/') || url.StartsWith(baseUri.AbsoluteUri)) &&
        !url.Equals(baseUri.AbsoluteUri);

    public static string NormalizeSourceUrl(string siteUrl) =>
        siteUrl.Contains("://") 
            ? (siteUrl.EndsWith('/') ? siteUrl : siteUrl + "/")
            : $"https://{siteUrl}/";
}
