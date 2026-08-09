namespace Sekmen.StaticSiteGenerator;

public static class Downloader
{
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
