namespace Sekmen.StaticSiteGenerator.Tests.Integration;

using Sekmen.StaticSiteGenerator.Tests.Helpers;

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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/404" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://static.example.com/";
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: targetUrl,
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        var indexContent = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        indexContent.ShouldContain(targetUrl);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldExtractAndDownloadResources()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/pdf-file" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act - should not crash when encountering PDF
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "pdf-file")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleMalformedHtmlCorrectly()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/malformed-html" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/circular-link-a" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
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
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/special-characters" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        File.Exists(Path.Combine(outputFolder, "special-characters", "index.html")).ShouldBeTrue();
        var content = File.ReadAllText(Path.Combine(outputFolder, "special-characters", "index.html"));
        content.ShouldContain("file-name_123.png");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ExportWebsite_ShouldHandleMissingResourcesGracefully()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/missing-resource" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act - should not crash when resources return 404
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - page should still be exported
        File.Exists(Path.Combine(outputFolder, "missing-resource", "index.html")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
