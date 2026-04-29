namespace Application.Smoke;

using System.Net;

using Application.Infrastructure;

/// <summary>
/// Smoke tests that verify the application boots successfully and the middleware
/// pipeline responds correctly to basic requests.
/// </summary>
public sealed class ApplicationSmokeTests(ApplicationFixture fixture) : ApplicationTestBase(fixture)
{
    [Fact]
    public async Task GET_OpenApi_WhenEnvironmentIsDevelopment_Returns200()
    {
        var response = await Client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_UnknownRoute_WhenNoEndpointMatched_Returns404()
    {
        var response = await Client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
