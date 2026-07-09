namespace Umbraco.Community.HtmlExporter.Composers;

public class UmbracoCommunityHtmlExporterApiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Related documentation:
        // https://docs.umbraco.com/umbraco-cms/extend-your-project/server-side-extensions/custom-backoffice-api
        // https://docs.umbraco.com/umbraco-cms/extend-your-project/server-side-extensions/api-versioning-and-openapi

        builder.AddBackOfficeOpenApiDocument(
            Constants.ApiName,
            document => document
                .WithTitle("Umbraco Community Html Exporter Backoffice API")
                .WithBackOfficeAuthentication()
                .WithJsonOptions(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice));
    }
}
