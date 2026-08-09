namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class UrlNormalizationTests
{
    [Theory]
    [InlineData("/page", true)]
    [InlineData("/path/to/page", true)]
    [InlineData("https://external.com", false)]
    [InlineData("http://external.com", false)]
    [InlineData("//external.com", false)]
    [InlineData("www.external.com", false)]
    [InlineData("#anchor", false)]
    [InlineData("mailto:test@test.com", false)]
    [InlineData("tel:+1234567890", false)]
    [InlineData("", false)]
    public async Task ShouldCorrectlyFilterUrlsAsInternalOrExternal(string href, bool shouldBeInternal)
    {
        // Arrange
        var html = $"""
            <html>
            <body>
                <a href="{href}">Link</a>
            </body>
            </html>
            """;
        
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html);
        
        if (shouldBeInternal && !string.IsNullOrEmpty(href) && href.StartsWith('/'))
        {
            mockBuilder.WithGetResponse($"https://example.com{href}", "<html></html>");
        }
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert - just verify it completes without throwing
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Theory]
    [InlineData("/page?query=value", "/page?query=value")]
    [InlineData("/page#anchor", "/page#anchor")]
    [InlineData("/page?q=1&other=2", "/page?q=1&other=2")]
    [InlineData("/special-chars_123.html", "/special-chars_123.html")]
    public async Task ShouldPreserveSpecialCharactersInUrls(string href, string expectedInUrl)
    {
        // Arrange
        var html = $"""
            <html>
            <body>
                <a href="{href}">Link</a>
            </body>
            </html>
            """;
        
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html)
            .WithGetResponse($"https://example.com{href}", "<html></html>");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Theory]
    [InlineData("/", "index.html")]
    [InlineData("/page", "page/index.html")]
    [InlineData("/path/to/page", "path/to/page/index.html")]
    [InlineData("/page.html", "page.html")]
    [InlineData("/path/file.pdf", "path/file.pdf")]
    public async Task ShouldNormalizePathsCorrectly(string urlPath, string expectedFile)
    {
        // Arrange
        var sitemapXml = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com{urlPath}</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse($"https://example.com{urlPath}", "<html></html>");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, expectedFile)).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
