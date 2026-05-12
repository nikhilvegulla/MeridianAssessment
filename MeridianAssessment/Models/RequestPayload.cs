namespace MeridianAssessment.Models;

public class RequestPayload
{
    public string? Type { get; set; }

    /// <summary>Layer answer (string). Large bodies are rejected by many gateways; use a compact proof when required.</summary>
    public string? Value { get; set; }

    public string? Notes { get; set; }
}
