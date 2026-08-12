namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class ErrorHandlingTests
{
    [Fact]
    public async Task ExportWebsite_WithMalformedSitemap_ShouldThrowException()
    {
        // Arrange
        const string malformedSitemap = """
                                        <?xml version="1.0"?>
                                        <urlset>
                                            <url>incomplete
                                        </urlset>
                                        """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", malformedSitemap, "application/xml");
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await Generator.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithMissingSitemap_ShouldThrowException()
    {
        // Arrange
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithNotFoundResponse("https://example.com/sitemap.xml");
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await Generator.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithMalformedHtml_ShouldContinueProcessing()
    {
        // Arrange
        const string malformedHtml = """
                                     <!DOCTYPE html>
                                     <html>
                                     <head>
                                         <title>Malformed
                                     <body>
                                         <h1>Incomplete HTML
                                         <a href="/about">Link</a>
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
            .WithGetResponse("https://example.com/", malformedHtml)
            .WithGetResponse("https://example.com/about", "<html></html>");
        
        HttpClient client = mockBuilder.Build();
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act - HtmlAgilityPack is forgiving with malformed HTML
        await Generator.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithPageReturningError_ShouldLogAndContinue()
    {
        // Arrange
        const string sitemapXml = """
                            <?xml version="1.0" encoding="UTF-8"?>
                            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                <url><loc>https://example.com/</loc></url>
                                <url><loc>https://example.com/error</loc></url>
                            </urlset>
                            """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", "<html><body></body></html>")
            .WithNotFoundResponse("https://example.com/error");
        
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
        
        // Assert - home page should still be exported
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithInvalidOutputFolder_ShouldThrowException()
    {
        // Arrange
        const string sitemapXml = """
                            <?xml version="1.0" encoding="UTF-8"?>
                            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                <url><loc>https://example.com/</loc></url>
                            </urlset>
                            """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", "<html></html>");
        
        HttpClient client = mockBuilder.Build();
        const string invalidFolder = @"\invalid?folder\path"; // Invalid path
        
        ExportCommand command = new(
            SiteUrl: "example.com",
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: invalidFolder,
            StringReplacements: []
        );
        
        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await Generator.ExportWebsite(client, command));
    }
    
    [Fact]
    public async Task ExportWebsite_WithInvalidTargetUrl_ShouldRewriteAnyway()
    {
        // Arrange
        const string html = """
                      <html>
                      <body>
                          <img src="https://example.com/image.jpg">
                          <img src="/image.jpg">
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
            TargetUrl: "https://static-bad-url", // No trailing slash
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Generator.ExportWebsite(client, command);
        
        // Assert - should still export despite non-standard target URL
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
