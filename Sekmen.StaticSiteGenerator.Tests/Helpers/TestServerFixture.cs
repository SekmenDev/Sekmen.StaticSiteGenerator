namespace Sekmen.StaticSiteGenerator.Tests.Helpers;

public class TestServerFixture : IAsyncLifetime
{
    private WebApplication? _app;
    public string BaseUrl { get; private set; } = string.Empty;
    
    public async Task InitializeAsync()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel(options =>
        {
            options.Listen(IPAddress.Loopback, 0);
        });
        _app = builder.Build();
        
        // Configure static file serving
        _app.UseStaticFiles();
        
        // Sitemap endpoint
        _app.MapGet("/sitemap.xml", GetSitemap);
        
        // Standard HTML pages
        _app.MapGet("/", GetIndexPage);
        _app.MapGet("/about", GetAboutPage);
        _app.MapGet("/contact", GetContactPage);
        _app.MapGet("/services", GetServicesPage);
        _app.MapGet("/blog/{slug}", GetBlogPage);
        _app.MapGet("/404", Get404Page);
        
        // Resource endpoints
        _app.MapGet("/css/{*path}", GetCssResource);
        _app.MapGet("/js/{*path}", GetJsResource);
        _app.MapGet("/images/{*path}", GetImageResource);
        _app.MapGet("/assets/{*path}", GetAssetResource);
        
        // Special test endpoints
        _app.MapGet("/malformed-html", GetMalformedHtml);
        _app.MapGet("/special-characters", GetSpecialCharacterPage);
        _app.MapGet("/circular-link-a", GetCircularPageA);
        _app.MapGet("/circular-link-b", GetCircularPageB);
        _app.MapGet("/missing-resource", GetPageWithMissingResource);
        _app.MapGet("/pdf-file", () => Results.File([], "application/pdf", "test.pdf"));
        
        // Start server on random port
        await _app.StartAsync();
        
        string addresses = _app.Urls.First();
        BaseUrl = addresses;
    }
    
    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
            await Task.Delay(100);
        }
    }
    
    private string GetSitemap() => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
            <url>
                <loc>{BaseUrl}/</loc>
            </url>
            <url>
                <loc>{BaseUrl}/about</loc>
            </url>
            <url>
                <loc>{BaseUrl}/services</loc>
            </url>
            <url>
                <loc>{BaseUrl}/blog/post-1</loc>
            </url>
        </urlset>
        """;
    
    private string GetIndexPage() => $"""
        <!DOCTYPE html>
        <html>
        <head>
            <title>Home</title>
            <link rel="stylesheet" href="/css/style.css">
            <script src="/js/app.js"></script>
        </head>
        <body>
            <h1>Welcome</h1>
            <img src="/images/logo.png" alt="Logo">
            <a href="/about">About</a>
            <a href="/services">Services</a>
            <a href="/blog/post-1">Blog Post</a>
            <a href="#section">Internal Link</a>
            <a href="mailto:test@example.com">Email</a>
            <a href="tel:+1234567890">Call Us</a>
            <a href="https://external.com">External</a>
        </body>
        </html>
        """;
    
    private string GetAboutPage() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>About</title>
            <link rel="stylesheet" href="/css/about.css">
        </head>
        <body>
            <h1>About Us</h1>
            <p>Company information</p>
            <a href="/">Home</a>
        </body>
        </html>
        """;
    
    private string GetContactPage() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Contact</title>
        </head>
        <body>
            <h1>Contact</h1>
            <a href="/">Home</a>
        </body>
        </html>
        """;
    
    private string GetServicesPage() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Services</title>
            <style>
                .hero { background-image: url('/images/hero.jpg'); }
            </style>
        </head>
        <body>
            <h1>Services</h1>
            <a href="/">Home</a>
        </body>
        </html>
        """;
    
    private string GetBlogPage(string slug) => $"""
        <!DOCTYPE html>
        <html>
        <head>
            <title>Blog - {slug}</title>
        </head>
        <body>
            <h1>Blog Post: {slug}</h1>
            <img src="/images/blog-{slug}.jpg" alt="Featured">
            <a href="/">Home</a>
        </body>
        </html>
        """;
    
    private string Get404Page() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>404 Not Found</title>
        </head>
        <body>
            <h1>404 - Page Not Found</h1>
        </body>
        </html>
        """;
    
    private string GetMalformedHtml() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Malformed
        <body>
            <h1>This HTML is malformed
            <a href="/about">About</a>
        </html>
        """;
    
    private string GetSpecialCharacterPage() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Special Chars</title>
        </head>
        <body>
            <h1>Special Characters & Entities</h1>
            <a href="/test?param=value&other=123">Query String</a>
            <a href="/test#anchor">With Anchor</a>
            <img src="/images/file-name_123.png" alt="Image">
        </body>
        </html>
        """;
    
    private string GetCircularPageA() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Circular A</title>
        </head>
        <body>
            <a href="/circular-link-b">Go to B</a>
        </body>
        </html>
        """;
    
    private string GetCircularPageB() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Circular B</title>
        </head>
        <body>
            <a href="/circular-link-a">Go to A</a>
        </body>
        </html>
        """;
    
    private string GetPageWithMissingResource() => """
        <!DOCTYPE html>
        <html>
        <head>
            <title>Missing Resources</title>
            <link rel="stylesheet" href="/css/nonexistent.css">
            <script src="/js/missing.js"></script>
        </head>
        <body>
            <h1>Missing Resources</h1>
            <img src="/images/nonexistent.png" alt="Missing">
        </body>
        </html>
        """;
    
    private IResult GetCssResource(string path) =>
        path.EndsWith("missing") ? Results.NotFound() : Results.Text("/* CSS */", "text/css");
    
    private IResult GetJsResource(string path) =>
        path.EndsWith("missing") ? Results.NotFound() : Results.Text("console.log('JS');", "application/javascript");
    
    private IResult GetImageResource(string path) =>
        path.EndsWith("nonexistent") ? Results.NotFound() : Results.File([0xFF, 0xD8], "image/jpeg");
    
    private IResult GetAssetResource(string path) => Results.Text("Asset content");
}
