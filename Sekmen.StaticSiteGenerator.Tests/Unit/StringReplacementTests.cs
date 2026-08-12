namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class StringReplacementTests
{
    [Fact]
    public async Task ExportWebsite_AppliesStringReplacementsToContent()
    {
        // Arrange
        const string html = """
                      <html>
                      <body>
                          <h1>Welcome to Umbraco CMS</h1>
                          <p>This is umbraco-cms content</p>
                      </body>
                      </html>
                      """;
        
        const string sitemapXml = """
                                  <?xml version="1.0" encoding="UTF-8"?>
                                  <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                      <url><loc>https://example.com/</loc></url>
                                  </urlset>
                                  """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html);
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements:
            [
                new StringReplacements("Umbraco CMS", "Umbraco"),
                new StringReplacements("umbraco-cms", "umbraco")
            ]
        );
        
        // Act
        await Generator.ExportWebsite(client, command);
        
        // Assert
        string exportedHtml = await File.ReadAllTextAsync(Path.Combine(outputFolder, "index.html"));
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
        const string sitemapXml = """
                                  <?xml version="1.0" encoding="UTF-8"?>
                                  <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                      <url><loc>https://example.com/umbraco-cms/page</loc></url>
                                  </urlset>
                                  """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/umbraco-cms/page", "<html></html>");
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements:
            [
                new StringReplacements("umbraco-cms", "umbraco")
            ]
        );
        
        // Act
        await Generator.ExportWebsite(client, command);
        
        // Assert - path should be normalized with replacement
        File.Exists(Path.Combine(outputFolder, "umbraco", "page", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithMultipleReplacements_AppliesInOrder()
    {
        // Arrange
        const string html = """
                      <html>
                      <body>
                          <p>This is OLD text OLD</p>
                      </body>
                      </html>
                      """;
        
        const string sitemapXml = """
                            <?xml version="1.0" encoding="UTF-8"?>
                            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                <url><loc>https://example.com/</loc></url>
                            </urlset>
                            """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html);
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements:
            [
                new StringReplacements("OLD", "TEMP"),
                new StringReplacements("TEMP", "NEW")
            ]
        );
        
        // Act
        await Generator.ExportWebsite(client, command);
        
        // Assert
        string exportedHtml = await File.ReadAllTextAsync(Path.Combine(outputFolder, "index.html"));
        exportedHtml.ShouldContain("This is NEW text NEW");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithEmptyReplacements_ShouldNotModifyContent()
    {
        // Arrange
        const string html = """
                      <html>
                      <body>
                          <h1>Original Content</h1>
                      </body>
                      </html>
                      """;
        
        const string sitemapXml = """
                                  <?xml version="1.0" encoding="UTF-8"?>
                                  <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                      <url><loc>https://example.com/</loc></url>
                                  </urlset>
                                  """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html);
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Generator.ExportWebsite(client, command);
        
        // Assert
        string exportedHtml = await File.ReadAllTextAsync(Path.Combine(outputFolder, "index.html"));
        exportedHtml.ShouldContain("Original Content");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
