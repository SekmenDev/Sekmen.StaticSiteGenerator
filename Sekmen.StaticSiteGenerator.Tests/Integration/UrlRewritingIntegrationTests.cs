namespace Sekmen.StaticSiteGenerator.Tests.Integration;

using Sekmen.StaticSiteGenerator.Tests.Helpers;

public class UrlRewritingIntegrationTests : IAsyncLifetime
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
    public async Task ShouldRewriteAbsoluteUrls()
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
        // The original URLs should be rewritten to target URL
        indexContent.ShouldContain(targetUrl);
        indexContent.ShouldNotContain(_fixture.BaseUrl);
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldRewriteRootRelativeUrls()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://cdn.example.com/";
        
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
    public async Task ShouldPreserveQueryStringsAfterRewriting()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://static.example.com/";
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/special-characters" },
            TargetUrl: targetUrl,
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        var content = File.ReadAllText(Path.Combine(outputFolder, "special-characters", "index.html"));
        content.ShouldContain("?param=value");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldPreserveAnchorsAfterRewriting()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://static.example.com/";
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: new[] { "/special-characters" },
            TargetUrl: targetUrl,
            OutputFolder: outputFolder,
            StringReplacements: Array.Empty<StringReplacements>()
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        var content = File.ReadAllText(Path.Combine(outputFolder, "special-characters", "index.html"));
        content.ShouldContain("#anchor");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldApplyStringReplacementsToExportedContent()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: new[]
            {
                new StringReplacements("Welcome", "Hello")
            }
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert
        var indexContent = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        indexContent.ShouldContain("Hello");
        indexContent.ShouldNotContain("Welcome");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldRewriteImgSrcUrls()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://cdn.example.com/";
        
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
        indexContent.ShouldContain($"{targetUrl}images/logo.png");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldRewriteScriptSrcUrls()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://cdn.example.com/";
        
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
        indexContent.ShouldContain($"{targetUrl}js/app.js");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldRewriteLinkHrefUrls()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://cdn.example.com/";
        
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
        indexContent.ShouldContain($"{targetUrl}css/style.css");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldRewriteInlineStyleBackgroundUrls()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var targetUrl = "https://cdn.example.com/";
        
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
        var servicesContent = File.ReadAllText(Path.Combine(outputFolder, "services", "index.html"));
        servicesContent.ShouldContain($"{targetUrl}images/hero.jpg");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
    
    [Fact]
    public async Task ShouldHandleMultipleConsecutiveStringReplacements()
    {
        // Arrange
        var outputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        var command = new ExportCommand(
            SiteUrl: _fixture.BaseUrl.Replace("https://", "").Replace("http://", "").TrimEnd('/'),
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: "https://static.example.com/",
            OutputFolder: outputFolder,
            StringReplacements: new[]
            {
                new StringReplacements("Welcome", "Hello"),
                new StringReplacements("Hello to", "Greetings to")
            }
        );
        
        // Act
        await Functions.ExportWebsite(_client!, command);
        
        // Assert - should apply replacements in order
        var indexContent = File.ReadAllText(Path.Combine(outputFolder, "index.html"));
        indexContent.ShouldContain("Hello");
        
        // Cleanup
        Directory.Delete(outputFolder, true);
    }
}
