namespace Sekmen.StaticSiteGenerator.Tests.Integration;

public class FullExportIntegrationTests : IAsyncLifetime
{
    private readonly TestServerFixture _fixture = new();
    private HttpClient? _client;
    
    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _client = new HttpClient();
    }
    
    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _fixture.DisposeAsync();
    }
    
    [Fact]
    public async Task ExportWebsite_WithRealServer_ShouldCrawlAndExportAllPages()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/404"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - verify all expected pages are exported
        File.Exists(Path.Combine(outputFolder, "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "about", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "services", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "blog", "post-1", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "404", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldRewriteUrlsInContent()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string targetUrl = "https://static.example.com/";
        
        ExportCommand command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: [],
            TargetUrl: targetUrl,
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        string indexContent = await File.ReadAllTextAsync(Path.Combine(outputFolder, "index.html"));
        indexContent.ShouldContain(targetUrl);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldExtractAndDownloadResources()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - check for resource files
        File.Exists(Path.Combine(outputFolder, "css", "style.css")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "css", "about.css")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "js", "app.js")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "images", "logo.png")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleInlineStyleBackgroundImages()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - hero.jpg should be downloaded from inline style background-image in services page
        File.Exists(Path.Combine(outputFolder, "images", "hero.jpg")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandlePdfFilesGracefully()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/pdf-file.pdf"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act - should not crash when encountering PDF
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "pdf-file.pdf")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleMalformedHtmlCorrectly()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/malformed-html"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - should still export despite malformed HTML
        File.Exists(Path.Combine(outputFolder, "malformed-html", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleCircularLinksWithoutHanging()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/circular-link-a"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act - should complete without hanging
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "circular-link-a", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "circular-link-b", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleSpecialCharactersInPaths()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/special-characters"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "special-characters", "index.html")).ShouldBeTrue();
        string content = await File.ReadAllTextAsync(Path.Combine(outputFolder, "special-characters", "index.html"));
        content.ShouldContain("file-name_123.png");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleMissingResourcesGracefully()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl,
            AdditionalUrls: ["/missing-resource"],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act - should not crash when resources return 404
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - page should still be exported
        File.Exists(Path.Combine(outputFolder, "missing-resource", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
