using MeridianAssessment.Enums;

namespace MeridianAssessment.Models;

public class RequestPayload
{
    public LevelType Type { get; set; }

    public string? Value { get; set; }

    public string? Notes { get; set; }
}
