namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Provides validation and normalization methods for identifying static resource files, internal links, and valid URLs.
/// </summary>
public static class UrlValidation
{
    private static readonly string[] ResourceExtensions = [".pdf", ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".svg", ".ico"];
    private static readonly string[] ExternalSchemes = ["//", "http://", "https://", "www."];
    private static readonly string[] ExcludedPrefixes = ["#", "mailto:", "tel:"];

    /// <summary>
    /// Determines whether the specified URL path ends with a recognized static resource extension (e.g. .pdf, .css, .js, images).
    /// </summary>
    /// <param name="urlPath">The relative or absolute URL path to evaluate.</param>
    /// <returns><c>true</c> if the path matches a static file extension; otherwise, <c>false</c>.</returns>
    public static bool IsResourceFile(string urlPath) =>
        ResourceExtensions.Any(ext => urlPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Determines whether a hyperlink target (<c>href</c>) represents a valid internal relative page link.
    /// </summary>
    /// <param name="href">The link attribute value to evaluate.</param>
    /// <returns><c>true</c> if the link is relative, starts with '/', and is not an anchor, external scheme, or protocol link; otherwise, <c>false</c>.</returns>
    public static bool IsInternalLink(string href) =>
        !string.IsNullOrWhiteSpace(href) &&
        !ExcludedPrefixes.Any(href.StartsWith) &&
        !ExternalSchemes.Any(href.StartsWith) &&
        href.StartsWith('/');

    /// <summary>
    /// Determines whether an extracted candidate URL is a valid resource URL that should be downloaded.
    /// </summary>
    /// <param name="url">The candidate URL string extracted from an HTML or CSS document.</param>
    /// <param name="baseUri">The base <see cref="Uri"/> of the host page.</param>
    /// <returns><c>true</c> if the URL is valid, relative to the host or matching the base URI, and not protocol-relative or self-referential; otherwise, <c>false</c>.</returns>
    public static bool IsValidResourceUrl(string url, Uri baseUri) =>
        !string.IsNullOrWhiteSpace(url) &&
        !url.StartsWith("//") &&
        (url.StartsWith('/') || url.StartsWith(baseUri.AbsoluteUri)) &&
        !url.Equals(baseUri.AbsoluteUri);

    /// <summary>
    /// Normalizes a site URL to ensure it has a scheme (defaulting to https://) and a trailing slash.
    /// </summary>
    /// <param name="siteUrl">The input site URL string.</param>
    /// <returns>The normalized site URL string.</returns>
    public static string NormalizeSourceUrl(string siteUrl) =>
        siteUrl.Contains("://") 
            ? (siteUrl.EndsWith('/') ? siteUrl : siteUrl + "/")
            : $"https://{siteUrl}/";
}
