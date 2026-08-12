namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Represents the configuration options required to perform a static site export.
/// </summary>
/// <param name="SiteUrl">The source website address or domain to crawl (e.g. "example.com" or "https://example.com/").</param>
/// <param name="AdditionalUrls">An array of additional relative or absolute URL paths to include in the export queue.</param>
/// <param name="TargetUrl">The target base URL used to rewrite links and asset references in the exported static site.</param>
/// <param name="OutputFolder">The local directory path where exported files and assets will be saved.</param>
/// <param name="StringReplacements">An array of custom ordered string replacements to apply to file paths and HTML content.</param>
public record ExportCommand(
    string SiteUrl,
    string[] AdditionalUrls,
    string TargetUrl,
    string OutputFolder,
    StringReplacements[] StringReplacements
);

/// <summary>
/// Represents a target pair of string values for custom search-and-replace transformations during site generation.
/// </summary>
/// <param name="OldValue">The target string value to search for and replace.</param>
/// <param name="NewValue">The replacement string value.</param>
public record StringReplacements(string OldValue, string NewValue);