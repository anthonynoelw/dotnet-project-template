namespace Src.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Base controller for API endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class Controller : ControllerBase
{
}
