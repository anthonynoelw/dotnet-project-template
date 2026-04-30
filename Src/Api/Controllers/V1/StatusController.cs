namespace Api.Controllers.V1;

using Asp.Versioning;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides basic API status information. Accessible at <c>GET /api/v1/status</c>.
/// </summary>
[ApiVersion("1.0")]
public sealed class StatusController : Controller
{
    /// <summary>
    /// Returns the current API health status and version.
    /// </summary>
    /// <returns>A 200 OK response with API status information.</returns>
    /// <response code="200">The API is healthy and the active version is returned.</response>
    [HttpGet]
    [ProducesResponseType<StatusResponse>(StatusCodes.Status200OK)]
    public IActionResult GetStatus() => Ok(new StatusResponse("healthy", "1.0"));
}
