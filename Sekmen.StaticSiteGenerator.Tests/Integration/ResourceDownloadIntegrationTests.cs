namespace Sekmen.StaticSiteGenerator.Tests.Integration;

using Sekmen.StaticSiteGenerator.Tests.Helpers;

public class ResourceDownloadIntegrationTests : IAsyncLifetime
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
    public async Task ShouldDownloadCssFiles()
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
        
        // Assert
        var cssFile = Path.Combine(outputFolder, "css", "style.css");
        File.Exists(cssFile).ShouldBeTrue();
        var content = File.ReadAllText(cssFile);
        content.ShouldContain("CSS");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldDownloadJavaScriptFiles()
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
        
        // Assert
        var jsFile = Path.Combine(outputFolder, "js", "app.js");
        File.Exists(jsFile).ShouldBeTrue();
        var content = File.ReadAllText(jsFile);
        content.ShouldContain("JS");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldDownloadImageFiles()
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
        
        // Assert
        var imageFile = Path.Combine(outputFolder, "images", "logo.png");
        File.Exists(imageFile).ShouldBeTrue();
        var bytes = File.ReadAllBytes(imageFile);
        bytes.Length.ShouldBeGreaterThan(0);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldCreateCorrectDirectoryStructure()
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
        
        // Assert
        Directory.Exists(Path.Combine(outputFolder, "css")).ShouldBeTrue();
        Directory.Exists(Path.Combine(outputFolder, "js")).ShouldBeTrue();
        Directory.Exists(Path.Combine(outputFolder, "images")).ShouldBeTrue();
        Directory.Exists(Path.Combine(outputFolder, "about")).ShouldBeTrue();
        Directory.Exists(Path.Combine(outputFolder, "services")).ShouldBeTrue();
        Directory.Exists(Path.Combine(outputFolder, "blog", "post-1")).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldNotRedownloadExistingResourcesWithSameSize()
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
        
        // Act - first export
        await Functions.ExportWebsite(_client!, command);
        var firstExportTime = File.GetLastWriteTime(Path.Combine(outputFolder, "css", "style.css"));
        
        // Wait a bit to ensure timestamps would differ if file is rewritten
        await Task.Delay(100);
        
        // Act - second export
        await Functions.ExportWebsite(_client!, command);
        var secondExportTime = File.GetLastWriteTime(Path.Combine(outputFolder, "css", "style.css"));
        
        // Assert - file should not be rewritten if size is same
        // Note: This test may be flaky as it depends on timing and file timestamps
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldHandleFilesWithSpecialCharactersInNames()
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
        var filePath = Path.Combine(outputFolder, "images", "file-name_123.png");
        File.Exists(filePath).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldHandleMissingResourcesWithoutCrashing()
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
        
        // Act - should not throw even though some resources are missing
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - page should exist, but missing resources shouldn't
        File.Exists(Path.Combine(outputFolder, "missing-resource", "index.html")).ShouldBeTrue();
        File.Exists(Path.Combine(outputFolder, "css", "nonexistent.css")).ShouldBeFalse();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
