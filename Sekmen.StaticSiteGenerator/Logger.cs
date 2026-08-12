namespace Sekmen.StaticSiteGenerator;

/// <summary>
/// Provides simple logging utilities for printing informational messages and errors to the console.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Writes an informational message to the standard console output.
    /// </summary>
    /// <param name="message">The text message to log.</param>
    public static void Info(string message) => Console.WriteLine(message);

    /// <summary>
    /// Writes an error message and optional exception information to the standard console output.
    /// </summary>
    /// <param name="message">The descriptive error message to log.</param>
    /// <param name="ex">An optional <see cref="Exception"/> instance associated with the error.</param>
    public static void Error(string message, Exception? ex = null) =>
        Console.WriteLine(ex != null ? $"{message}: {ex.Message}" : message);
}