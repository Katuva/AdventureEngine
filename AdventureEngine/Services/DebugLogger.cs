namespace AdventureEngine.Services;

/// <summary>
/// Helper class for conditional debug output
/// </summary>
public static class DebugLogger
{
    public static bool IsEnabled { get; set; }

    public static void Log(string message)
    {
        if (IsEnabled)
        {
            Console.WriteLine($"[DEBUG] {message}");
        }
    }
}
