namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Provides functionality for downloading static asset and resource files from remote HTTP endpoints.
/// </summary>
public static class Downloader
{
    /// <summary>
    /// Downloads a resource file directly from the specified URL and saves it to the output directory.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used to make HTTP requests.</param>
    /// <param name="pageUrl">The full absolute URL of the resource file to download.</param>
    /// <param name="uri">The <see cref="Uri"/> representing the resource location, used to construct the local file path.</param>
    /// <param name="outputFolder">The directory path where the downloaded file should be saved.</param>
    /// <returns>A task representing the asynchronous download operation.</returns>
    public static async Task DownloadResourceFile(HttpClient client, string pageUrl, Uri uri, string outputFolder)
    {
        try
        {
            byte[] fileContent = await client.GetByteArrayAsync(pageUrl);
            string filePath = Path.Combine(outputFolder, uri.AbsolutePath.TrimStart('/'));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllBytesAsync(filePath, fileContent);
            Logger.Info($"File downloaded: {filePath}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to download resource {pageUrl}", ex);
        }
    }

    /// <summary>
    /// Resolves a resource URL relative to a parent page URI and downloads the file if it has not already been saved.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance used to make HTTP requests.</param>
    /// <param name="pageUri">The base <see cref="Uri"/> of the page containing the resource reference.</param>
    /// <param name="resourceUrl">The relative or absolute URL of the asset referenced within the page.</param>
    /// <param name="outputFolder">The directory path where the downloaded file should be saved.</param>
    /// <returns>A task representing the asynchronous download operation.</returns>
    public static async Task DownloadResource(HttpClient client, Uri pageUri, string resourceUrl, string outputFolder)
    {
        try
        {
            Uri resourceUri = new(pageUri, resourceUrl);
            string resourcePath = Path.Combine(outputFolder, resourceUri.AbsolutePath.TrimStart('/'));
            Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

            if (File.Exists(resourcePath))
                return;

            byte[] data = await client.GetByteArrayAsync(resourceUri);
            await File.WriteAllBytesAsync(resourcePath, data);
            Logger.Info($"Downloaded: {resourceUri}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed: {resourceUrl}", ex);
        }
    }
}
