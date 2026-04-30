namespace Api.Extensions;

using Api.Exceptions;

/// <summary>
/// Extension methods for registering application services on <see cref="IHostApplicationBuilder"/>.
/// </summary>
internal static class ServiceExtensions
{
    /// <summary>
    /// Registers all API services: controllers, OpenAPI document generation,
    /// RFC 9457 problem details, and the global exception handler.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <returns>The same <paramref name="builder"/> instance for chaining.</returns>
    internal static IHostApplicationBuilder AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }
}
