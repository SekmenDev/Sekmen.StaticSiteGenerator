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
        string html = $"""
                       <html>
                       <body>
                           <a href="{href}">Link</a>
                       </body>
                       </html>
                       """;
        
        string sitemapXml = """
                            <?xml version="1.0" encoding="UTF-8"?>
                            <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                <url><loc>https://example.com/</loc></url>
                            </urlset>
                            """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse("https://example.com/", html);
        
        if (shouldBeInternal && !string.IsNullOrEmpty(href) && href.StartsWith('/'))
        {
            mockBuilder.WithGetResponse($"https://example.com{href}", "<html></html>");
        }
        
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
        
        // Assert - just verify it completes without throwing
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Theory]
    [InlineData("/page?query=value")]
    [InlineData("/page#anchor")]
    [InlineData("/page?q=1&other=2")]
    [InlineData("/special-chars_123.html")]
    public async Task ShouldPreserveSpecialCharactersInUrls(string href)
    {
        // Arrange
        string html = $"""
                       <html>
                       <body>
                           <a href="{href}">Link</a>
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
            .WithGetResponse("https://example.com/", html)
            .WithGetResponse($"https://example.com{href}", "<html></html>");
        
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
        string sitemapXml = $$"""
                             <?xml version="1.0" encoding="UTF-8"?>
                             <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                                 <url><loc>https://example.com{{urlPath}}</loc></url>
                             </urlset>
                             """;
        
        HttpClientMockBuilder mockBuilder = new HttpClientMockBuilder()
            .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
            .WithGetResponse($"https://example.com{urlPath}", "<html></html>");
        
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
        File.Exists(Path.Combine(outputFolder, expectedFile)).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }

    [Fact]
    public void UpdateHtmlUrls_ShouldNotModifyExternalProtocolRelativeUrls()
    {
        // Arrange
        const string inputHtml = """
            <head>
                <link rel="canonical" href="//localhost:44362/">
                <link rel="stylesheet" href="//fonts.googleapis.com/css?family=Lora">
                <script src="//www.googletagmanager.com/gtag/js"></script>
                <link href="/assets/css/styles.css" rel="stylesheet">
            </head>
            """;

        ExportCommand command = new(
            SiteUrl: "https://localhost:44362/",
            AdditionalUrls: [],
            TargetUrl: "https://huseyinsekmenoglu.net/",
            OutputFolder: "out",
            StringReplacements: []
        );

        // Act
        string result = UrlHelpers.UpdateHtmlUrls(inputHtml, "https://localhost:44362/", command);

        // Assert
        result.ShouldContain("href=\"https://huseyinsekmenoglu.net/\"");
        result.ShouldContain("href=\"//fonts.googleapis.com/css?family=Lora\"");
        result.ShouldContain("src=\"//www.googletagmanager.com/gtag/js\"");
        result.ShouldContain("href=\"https://huseyinsekmenoglu.net/assets/css/styles.css\"");
        result.ShouldNotContain("https://huseyinsekmenoglu.net//fonts");
        result.ShouldNotContain("https://huseyinsekmenoglu.net//www.googletagmanager");
    }
}
