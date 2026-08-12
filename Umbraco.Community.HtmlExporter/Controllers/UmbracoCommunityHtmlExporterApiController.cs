namespace Umbraco.Community.HtmlExporter.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Umbraco.Community.HtmlExporter")]
[MapToApi(Constants.ApiName)]
public class UmbracoCommunityHtmlExporterApiController(
    IHttpClientFactory httpClientFactory,
    IUmbracoContextFactory umbracoContextFactory,
    IDocumentNavigationQueryService documentNavigationQueryService,
    IDomainService domainService,
    IOptions<ExportHtmlSettings> exportHtmlSettings
) : UmbracoCommunityHtmlExporterApiControllerBase
{
    [HttpGet("get-data")]
    [ProducesResponseType<DashboardViewModel>(StatusCodes.Status200OK)]
    public async Task<DashboardViewModel> GetData()
    {
        IEnumerable<IDomain> domains = await domainService.GetAllAsync(true);
        UmbracoContextReference umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext();
        documentNavigationQueryService.TryGetRootKeys(out IEnumerable<Guid> rootKeys);
        DashboardDomains[] domainsArray = rootKeys
            .Select(key => umbracoContextReference.UmbracoContext.Content.GetById(key))
            .WhereNotNull()
            .Select(m => new DashboardDomains(
                m.Id,
                m.Name,
                domains.FirstOrDefault(n => n.RootContentId == m.Id)?.DomainName ?? "No domain assigned")
            )
            .ToArray();

        return new DashboardViewModel(domainsArray, exportHtmlSettings.Value);
    }

    [HttpPost("export-website")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportWebsite([FromForm] ExportCommand request)
    {
        using HttpClient client = httpClientFactory.CreateClient();

        await Generator.ExportWebsite(client, request);

        return Content("Done.");
    }
}