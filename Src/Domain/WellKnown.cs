namespace Domain;

/// <summary>Shared string constants used across layers to avoid magic strings.</summary>
public static class WellKnown
{
    /// <summary>
    /// Tag constants used to categorise health checks by probe type.
    /// Attach these when registering infrastructure checks:
    /// <code>
    /// builder.Services.AddHealthChecks()
    ///     .AddDbContextCheck&lt;AppDbContext&gt;(tags: [HealthCheckTags.Ready]);
    /// </code>
    /// </summary>
    public static class HealthCheckTags
    {
        /// <summary>Marks a check as a readiness dependency. Runs under <c>GET /health/ready</c>.</summary>
        public const string Ready = "ready";
    }
}
