namespace Api;

using Api.Extensions;

using Serilog;

/// <summary>
/// Entry point for the API application.
/// </summary>
public partial class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    private static void Main(string[] args)
    {
        // Captures fatal errors during host construction before appsettings.json is loaded.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.AddSerilogLogging();
            builder.AddApiServices();

            WebApplication app = builder.Build();

            // Must precede UseExceptionHandler: wraps the full pipeline so Serilog
            // captures the final status code for exception-mapped 5xx responses.
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                };
            });

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
