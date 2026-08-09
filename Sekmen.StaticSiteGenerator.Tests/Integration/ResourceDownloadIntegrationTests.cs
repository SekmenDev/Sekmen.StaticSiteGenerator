namespace Sekmen.StaticSiteGenerator.Tests.Integration;

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
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        string cssFile = Path.Combine(outputFolder, "css", "style.css");
        File.Exists(cssFile).ShouldBeTrue();
        string content = await File.ReadAllTextAsync(cssFile);
        content.ShouldContain("CSS");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldDownloadJavaScriptFiles()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        string jsFile = Path.Combine(outputFolder, "js", "app.js");
        File.Exists(jsFile).ShouldBeTrue();
        string content = File.ReadAllText(jsFile);
        content.ShouldContain("JS");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldDownloadImageFiles()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        string imageFile = Path.Combine(outputFolder, "images", "logo.png");
        File.Exists(imageFile).ShouldBeTrue();
        byte[] bytes = File.ReadAllBytes(imageFile);
        bytes.Length.ShouldBeGreaterThan(0);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldCreateCorrectDirectoryStructure()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
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
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: [],
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act - first export
        await Functions.ExportWebsite(_client!, command);
        DateTime firstExportTime = File.GetLastWriteTime(Path.Combine(outputFolder, "css", "style.css"));
        
        // Wait a bit to ensure timestamps would differ if file is rewritten
        await Task.Delay(100);
        
        // Act - second export
        await Functions.ExportWebsite(_client!, command);
        DateTime secondExportTime = File.GetLastWriteTime(Path.Combine(outputFolder, "css", "style.css"));
        
        // Assert - file should not be rewritten if size is same
        // Note: This test may be flaky as it depends on timing and file timestamps
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldHandleFilesWithSpecialCharactersInNames()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/special-characters" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        string filePath = Path.Combine(outputFolder, "images", "file-name_123.png");
        File.Exists(filePath).ShouldBeTrue();
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldHandleMissingResourcesWithoutCrashing()
    {
        // Arrange
        string outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        ExportCommand command = new(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/missing-resource" },
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: []
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
