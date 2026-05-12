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

    /// <summary>
    /// Submits qualitative analysis. Use <paramref name="notes"/> for supporting detail (respect API size cap, e.g. ≤ 8 KiB).
    /// </summary>
    /// <param name="notes">Optional extended notes.</param>
    public Task<RequestPayload?> ProcessAsync(string? notes = null)
    {
        
    }
}
