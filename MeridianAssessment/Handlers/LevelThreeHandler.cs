using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;

namespace MeridianAssessment.Handlers;

/// <summary>
/// Layer 3: find the short alphabetic answer hidden in decrypted records and submit it in <see cref="RequestPayload.Value"/>.
/// </summary>
public class LevelThreeHandler
{
    private readonly IMeridianClient _client;

    public LevelThreeHandler(IMeridianClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Searches decrypted content and submits the layer-3 answer.
    /// </summary>
    /// <param name="decryptedDataset">Optional decrypted text or JSON from layer 2 for local search.</param>
    public Task<RequestPayload?> ProcessAsync(string? decryptedDataset = null)
    {
        
    }
}
