namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class StringReplacementTests
{
    [Fact]
    public async Task ExportWebsite_AppliesStringReplacementsToContent()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <h1>Welcome to Umbraco CMS</h1>
                <p>This is umbraco-cms content</p>
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
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: new[]
            {
                new StringReplacements("Umbraco CMS", "Umbraco"),
                new StringReplacements("umbraco-cms", "umbraco")
            }
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        var exportedHtml = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        exportedHtml.ShouldContain("Welcome to Umbraco");
        exportedHtml.ShouldContain("This is umbraco content");
        exportedHtml.ShouldNotContain("Umbraco CMS");
        exportedHtml.ShouldNotContain("umbraco-cms");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_AppliesStringReplacementsToFilePaths()
    {
        // Arrange
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/umbraco-cms/page</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/umbraco-cms/page", "<html></html>");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: new[]
            {
                new StringReplacements("umbraco-cms", "umbraco")
            }
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert - path should be normalized with replacement
        File.Exists(Path.Combine(outputFolder, "umbraco", "page", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithMultipleReplacements_AppliesInOrder()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <p>This is OLD text OLD</p>
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
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: new[]
            {
                new StringReplacements("OLD", "TEMP"),
                new StringReplacements("TEMP", "NEW")
            }
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        var exportedHtml = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        exportedHtml.ShouldContain("This is NEW text NEW");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithEmptyReplacements_ShouldNotModifyContent()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <h1>Original Content</h1>
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
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        var exportedHtml = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        exportedHtml.ShouldContain("Original Content");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
