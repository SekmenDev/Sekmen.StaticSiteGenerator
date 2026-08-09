namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class ErrorHandlingTests
{
    [Fact]
    public async Task ExportWebsite_WithMalformedSitemap_ShouldThrowException()
    {
        // Arrange
        var malformedSitemap = """
            <?xml version="1.0"?>
            <urlset>
                <url>incomplete
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", malformedSitemap, "application/xml");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await Functions.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithMissingSitemap_ShouldThrowException()
    {
        // Arrange
        var mockBuilder = new HttpClientMockBuilder()
            .WithNotFoundResponse("https://example.com/sitemap.xml");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await Functions.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithMalformedHtml_ShouldContinueProcessing()
    {
        // Arrange
        var malformedHtml = """
            <!DOCTYPE html>
            <html>
            <head>
                <title>Malformed
            <body>
                <h1>Incomplete HTML
                <a href="/about">Link</a>
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
            .WithGetResponse("https://example.com/", malformedHtml)
            .WithGetResponse("https://example.com/about", "<html></html>");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act - HtmlAgilityPack is forgiving with malformed HTML
        await Functions.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithPageReturningError_ShouldLogAndContinue()
    {
        // Arrange
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/</loc></url>
                <url><loc>https://example.com/error</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", "<html><body></body></html>")
            .WithNotFoundResponse("https://example.com/error");
        
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
        
        // Assert - home page should still be exported
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithInvalidOutputFolder_ShouldThrowException()
    {
        // Arrange
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", "<html></html>");
        
        var client = mockBuilder.Build();
        var invalidFolder = "\\invalid?folder\\path"; // Invalid path
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: invalidFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await Functions.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithInvalidTargetUrl_ShouldRewriteAnyway()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <img src="https://example.com/image.jpg">
                <img src="/image.jpg">
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
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static-bad-url", // No trailing slash
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert - should still export despite non-standard target URL
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
