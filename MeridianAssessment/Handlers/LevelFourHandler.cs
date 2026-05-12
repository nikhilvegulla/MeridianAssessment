using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;

namespace MeridianAssessment.Handlers;

/// <summary>
/// Layer 4: free-form analysis scored by a human; multiple submissions are allowed.
/// </summary>
public class LevelFourHandler
{
    private readonly IMeridianClient _client;

    public LevelFourHandler(IMeridianClient client)
    {
        _client = client;
    }

    public Task<RequestPayload?> ProcessAsync(string? notes = null)
    {
        return null;
    }
}
