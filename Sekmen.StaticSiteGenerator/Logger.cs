namespace Sekmen.StaticSiteGenerator;

public static class Logger
{
    public static void Info(string message) => Console.WriteLine(message);
    public static void Error(string message, Exception? ex = null) =>
        Console.WriteLine(ex != null ? $"{message}: {ex.Message}" : message);
}