namespace Api.Controllers.V1;

/// <summary>Represents the API status response payload.</summary>
/// <param name="Status">Current operational status of the API.</param>
/// <param name="Version">The API version serving this response.</param>
public sealed record StatusResponse(string Status, string Version);
