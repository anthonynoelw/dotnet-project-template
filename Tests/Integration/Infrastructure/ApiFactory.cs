namespace Integration.Infrastructure;

using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TEntryPoint}"/> that boots the API in Development
/// mode and registers test-only endpoints from the Integration assembly.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Register the Integration assembly as an application part so ASP.NET Core
            // discovers TestController alongside the production controllers.
            services.AddMvc()
                .AddApplicationPart(typeof(ApiFactory).Assembly);
        });
    }
}
