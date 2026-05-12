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

    public Task<RequestPayload?> ProcessAsync(string? decryptedDataset = null)
    {
        return null;
    }
}
