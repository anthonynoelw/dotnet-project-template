namespace Agent;

using Agent;

/// <summary>
/// Entry point for the Agent application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point for the Agent application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}
