# Sekmen.StaticSiteGenerator - Test Suite Documentation

## Overview

Comprehensive test suite for the **Sekmen.StaticSiteGenerator** core library using **xUnit**, **Shouldly**, and **Moq**. Tests cover unit, integration, and end-to-end scenarios with full feature coverage and edge case handling.

## Test Project Structure

```
Sekmen.StaticSiteGenerator.Tests/
├── GlobalUsings.cs                    # Shared imports
├── Helpers/
│   ├── TestServerFixture.cs          # ASP.NET Core Minimal API test server
│   └── HttpClientMockBuilder.cs      # Fluent mock HttpClient builder
├── Unit/
│   ├── ExportWebsiteTests.cs         # Core export logic (6 tests)
│   ├── UrlNormalizationTests.cs      # URL filtering & path normalization (4 tests)
│   ├── StringReplacementTests.cs     # Content & path replacements (4 tests)
│   └── ErrorHandlingTests.cs         # Exception & error scenarios (6 tests)
└── Integration/
    ├── FullExportIntegrationTests.cs       # End-to-end export (8 tests)
    ├── ResourceDownloadIntegrationTests.cs # Asset downloads (7 tests)
    └── UrlRewritingIntegrationTests.cs     # URL rewriting (10 tests)
```

**Total: 45 tests** covering features, edge cases, and error scenarios.

---

## Unit Tests (20 tests)

### ExportWebsiteTests.cs (6 tests)
Tests the core `Functions.ExportWebsite()` method with mocked HTTP responses.

| Test | Purpose |
|------|---------|
| `ExportWebsite_WithValidSitemap_ShouldLoadAndParseUrls` | Sitemap parsing and URL queuing |
| `ExportWebsite_WithAdditionalUrls_ShouldIncludeThem` | Additional URLs injection |
| `ExportWebsite_WithExternalLinks_ShouldIgnoreThem` | External link filtering |
| `ExportWebsite_WithAnchorAndMailtoLinks_ShouldIgnoreThem` | Special link types (#, mailto:, tel:) |
| `ExportWebsite_WithCircularLinks_ShouldNotHangDueToVisitedTracking` | Circular link handling |
| `ExportWebsite_WithMissingPageUrl_ShouldHandleGracefully` | 404 handling |

### UrlNormalizationTests.cs (4 tests)
Tests URL validation and path normalization logic.

| Test | Purpose |
|------|---------|
| `ShouldCorrectlyFilterUrlsAsInternalOrExternal` (parameterized) | URL classification for various schemes |
| `ShouldPreserveSpecialCharactersInUrls` (parameterized) | Query strings, anchors, underscores |
| `ShouldNormalizePathsCorrectly` (parameterized) | Path to file mapping (/page → page/index.html) |

### StringReplacementTests.cs (4 tests)
Tests text replacement in content and file paths.

| Test | Purpose |
|------|---------|
| `ExportWebsite_AppliesStringReplacementsToContent` | HTML content replacement |
| `ExportWebsite_AppliesStringReplacementsToFilePaths` | File path normalization |
| `ExportWebsite_WithMultipleReplacements_AppliesInOrder` | Chained replacements |
| `ExportWebsite_WithEmptyReplacements_ShouldNotModifyContent` | No-op behavior |

### ErrorHandlingTests.cs (6 tests)
Tests exception handling and error scenarios.

| Test | Purpose |
|------|---------|
| `ExportWebsite_WithMalformedSitemap_ShouldThrowException` | Invalid XML |
| `ExportWebsite_WithMissingSitemap_ShouldThrowException` | 404 sitemap.xml |
| `ExportWebsite_WithMalformedHtml_ShouldContinueProcessing` | HtmlAgilityPack forgiveness |
| `ExportWebsite_WithPageReturningError_ShouldLogAndContinue` | Partial failure handling |
| `ExportWebsite_WithInvalidOutputFolder_ShouldThrowException` | Path validation |
| `ExportWebsite_WithInvalidTargetUrl_ShouldRewriteAnyway` | Malformed target URL |

---

## Integration Tests (25 tests)

### FullExportIntegrationTests.cs (8 tests)
End-to-end tests using real `TestServerFixture` serving dynamic HTML.

| Test | Purpose |
|------|---------|
| `ExportWebsite_WithRealServer_ShouldCrawlAndExportAllPages` | Full site export with 5 pages |
| `ExportWebsite_ShouldRewriteUrlsInContent` | URL replacement in HTML |
| `ExportWebsite_ShouldExtractAndDownloadResources` | Asset discovery & download |
| `ExportWebsite_ShouldHandleInlineStyleBackgroundImages` | Regex extraction from style attr |
| `ExportWebsite_ShouldHandlePdfFilesGracefully` | PDF content skipping |
| `ExportWebsite_ShouldHandleMalformedHtmlCorrectly` | Malformed HTML parsing |
| `ExportWebsite_ShouldHandleCircularLinksWithoutHanging` | Circular reference prevention |
| `ExportWebsite_ShouldHandleSpecialCharactersInPaths` | Unicode & special chars |
| `ExportWebsite_ShouldHandleMissingResourcesGracefully` | Missing asset handling (404) |

### ResourceDownloadIntegrationTests.cs (7 tests)
Tests asset discovery, download, and file system operations.

| Test | Purpose |
|------|---------|
| `ShouldDownloadCssFiles` | CSS file download |
| `ShouldDownloadJavaScriptFiles` | JS file download |
| `ShouldDownloadImageFiles` | Image file download |
| `ShouldCreateCorrectDirectoryStructure` | Nested directory creation |
| `ShouldNotRedownloadExistingResourcesWithSameSize` | Conditional download (size check) |
| `ShouldHandleFilesWithSpecialCharactersInNames` | File naming edge cases |
| `ShouldHandleMissingResourcesWithoutCrashing` | 404 assets |

### UrlRewritingIntegrationTests.cs (10 tests)
Tests URL rewriting in exported HTML.

| Test | Purpose |
|------|---------|
| `ShouldRewriteAbsoluteUrls` | Protocol + domain replacement |
| `ShouldRewriteRootRelativeUrls` | /path rewriting |
| `ShouldPreserveQueryStringsAfterRewriting` | ?param=value preservation |
| `ShouldPreserveAnchorsAfterRewriting` | #anchor preservation |
| `ShouldApplyStringReplacementsToExportedContent` | Text replacement post-rewrite |
| `ShouldRewriteImgSrcUrls` | Image tag rewriting |
| `ShouldRewriteScriptSrcUrls` | Script tag rewriting |
| `ShouldRewriteLinkHrefUrls` | Link tag rewriting |
| `ShouldRewriteInlineStyleBackgroundUrls` | CSS background-image rewriting |
| `ShouldHandleMultipleConsecutiveStringReplacements` | Chained replacements |

---

## Test Helpers

### TestServerFixture.cs
Provides a real ASP.NET Core Minimal API test server that serves:
- **Pages**: Home, About, Services, Blog, Contact, 404, malformed, special chars, circular links
- **Resources**: CSS, JS, images, assets  
- **Scenarios**: Missing resources, PDFs, special characters, circular links

Implements `IAsyncLifetime` for automatic setup/teardown per test class.

**Key Features**:
- Random port assignment (no port conflicts)
- Dynamic Sitemap generation
- 404 response support for missing resources
- Inline style URL extraction testing

### HttpClientMockBuilder.cs
Fluent builder for mocking HttpClient with GET/HEAD responses.

**Usage**:
```csharp
var mockBuilder = new HttpClientMockBuilder()
    .WithGetResponse("https://example.com/sitemap.xml", sitemapXml, "application/xml")
    .WithGetResponse("https://example.com/page", htmlContent)
    .WithNotFoundResponse("https://example.com/missing")
    .WithHeadResponse("https://example.com/image.jpg", 12345);

var client = mockBuilder.Build();
```

---

## Running Tests

### All Tests
```bash
cd Sekmen.StaticSiteGenerator.Tests
dotnet test
```

### Unit Tests Only
```bash
dotnet test --filter "Namespace=Sekmen.StaticSiteGenerator.Tests.Unit"
```

### Integration Tests Only
```bash
dotnet test --filter "Namespace=Sekmen.StaticSiteGenerator.Tests.Integration"
```

### Specific Test
```bash
dotnet test --filter "Name=ExportWebsite_WithValidSitemap_ShouldLoadAndParseUrls"
```

### With Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

---

## Test Coverage

**45 Total Tests** covering:

### Features ✓
- [x] Sitemap parsing and URL discovery
- [x] Internal link crawling & URL validation
- [x] External link filtering (protocols, domains, schemes)
- [x] Resource extraction (links, scripts, images, inline styles)
- [x] Asset download with conditional logic (size comparison)
- [x] URL rewriting (absolute, root-relative, inline styles)
- [x] String replacements (content & paths)
- [x] Directory creation & file I/O
- [x] PDF file handling
- [x] Additional URL injection

### Edge Cases ✓
- [x] Circular link detection & prevention
- [x] Missing/404 resources
- [x] Missing/404 pages
- [x] Malformed HTML (HtmlAgilityPack recovery)
- [x] Malformed XML/Sitemap
- [x] Special characters in URLs/paths
- [x] Query strings & anchors preservation
- [x] Inline style background-image extraction (regex)
- [x] Anchor links (#) filtering
- [x] mailto: & tel: link filtering

### Error Handling ✓
- [x] Missing sitemap.xml (HTTP 404)
- [x] Malformed sitemap.xml (XML parse error)
- [x] Invalid output folder paths
- [x] Network errors (mocked)
- [x] 404 pages (graceful continue)
- [x] 404 assets (graceful continue)
- [x] Mixed protocols (http/https)
- [x] Protocol-relative URLs (//)
- [x] Invalid target URLs

---

## Notes

- **Unit tests** use mocked HttpClient for fast execution (~400ms total)
- **Integration tests** use real `TestServerFixture` for realistic scenarios
- Each integration test class gets its own server instance (IAsyncLifetime)
- Shouldly assertions provide clear failure messages
- Moq enables precise request mocking for edge case testing
- Tests clean up temp folders automatically

---

## Architecture Decisions

1. **Separate Unit & Integration Folders**: Clear separation of concerns (mocked vs. real)
2. **TestServerFixture per Class**: Isolates server state, prevents port conflicts
3. **HttpClientMockBuilder**: Simplifies mock setup for complex scenarios
4. **Parameterized Tests**: Reduces duplication for similar test cases
5. **IAsyncLifetime**: Ensures proper async setup/teardown without manual disposal

---

## Future Enhancements

- [ ] Performance benchmarks for large sites
- [ ] Concurrent crawling tests (when parallelism is added)
- [ ] SSL/certificate testing
- [ ] Proxy handling tests
- [ ] Rate limiting simulation
- [ ] Memory leak detection in circular scenarios
