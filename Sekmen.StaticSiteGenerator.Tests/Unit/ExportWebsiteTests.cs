namespace Sekmen.StaticSiteGenerator.Tests.Unit;

public class ExportWebsiteTests
{
    [Fact]
    public async Task ExportWebsite_WithValidSitemap_ShouldLoadAndParseUrls()
    {
        // Arrange
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/page1</loc></url>
                <url><loc>https://example.com/page2</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/page1", "<html><body><a href='/page2'>Link</a></body></html>")
            .WithGetResponse("https://example.com/page2", "<html><body></body></html>");
        
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
        Directory.Exists(outputFolder).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithAdditionalUrls_ShouldIncludeThem()
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
            .WithGetResponse("https://example.com/", "<html><body></body></html>")
            .WithGetResponse("https://example.com/404", "<html><body>Not Found</body></html>")
            .WithGetResponse("https://example.com/robots.txt", "User-agent: *");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: new[] { "/404", "/robots.txt" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "404", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "robots.txt")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithExternalLinks_ShouldIgnoreThem()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <a href="https://external.com/page">External</a>
                <a href="//cdn.example.com/resource">Protocol Relative</a>
                <a href="http://example.com/internal">HTTP</a>
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
        
        // Assert - should only have exported the home page
        Directory.EnumerateFiles(outputFolder, "*", SearchOption.AllDirectories).Count().ShouldBe(1);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithAnchorAndMailtoLinks_ShouldIgnoreThem()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <a href="#section">Anchor</a>
                <a href="mailto:test@example.com">Email</a>
                <a href="tel:+1234567890">Phone</a>
                <a href="/about">Valid</a>
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
            .WithGetResponse("https://example.com/about", "<html><body></body></html>");
        
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
        File.Exists(Path.Combine(outputFolder, "about", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithCircularLinks_ShouldNotHangDueToVisitedTracking()
    {
        // Arrange
        var pageA = """
            <html>
            <body>
                <a href="/page-b">Go to B</a>
            </body>
            </html>
            """;
        
        var pageB = """
            <html>
            <body>
                <a href="/page-a">Go to A</a>
            </body>
            </html>
            """;
        
        var sitemapXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                <url><loc>https://example.com/page-a</loc></url>
            </urlset>
            """;
        
        var mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/page-a", pageA)
            .WithGetResponse("https://example.com/page-b", pageB);
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act - should complete without hanging
        await Functions.ExportWebsite(client, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "page-a", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "page-b", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_WithMissingPageUrl_ShouldHandleGracefully()
    {
        // Arrange
        var html = """
            <html>
            <body>
                <a href="/missing">Missing Page</a>
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
            .WithNotFoundResponse("https://example.com/missing");
        
        var client = mockBuilder.Build();
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: "example.com",
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act - should handle 404 gracefully
        await Functions.ExportWebsite(client, command);
        
        // Assert - home page should still be exported
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
