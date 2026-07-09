namespace Umbraco.Community.HtmlExporter.Controllers;

[ApiController]
[BackOfficeRoute("umbracocommunityhtmlexporter/api/v{version:apiVersion}")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(Constants.ApiName)]
[JsonOptionsName(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice)]
public class UmbracoCommunityHtmlExporterApiControllerBase : ControllerBase;
