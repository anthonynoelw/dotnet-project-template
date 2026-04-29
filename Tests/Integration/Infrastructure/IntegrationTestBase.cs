namespace Integration.Infrastructure;

using Microsoft.AspNetCore.Mvc.Testing;

/// <summary>
/// Base class for integration tests. Declares <see cref="IClassFixture{TFixture}"/> so xUnit
/// creates one <see cref="ApiFactory"/> per test class and provides an <see cref="HttpClient"/>
/// that targets the in-process test server.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<ApiFactory>
{
    /// <summary>Initializes a new instance of <see cref="IntegrationTestBase"/>.</summary>
    protected IntegrationTestBase(ApiFactory factory)
    {
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    /// <summary>Gets the HTTP client targeting the in-process test server.</summary>
    protected HttpClient Client { get; }
}
