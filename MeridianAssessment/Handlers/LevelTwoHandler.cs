using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;

namespace MeridianAssessment.Handlers;

/// <summary>
/// Layer 2: decrypt the dataset using the platform-issued key (see <see cref="IMeridianClient.GetSecretKey"/> and cached material).
/// </summary>
public class LevelTwoHandler
{
    private readonly IMeridianClient _client;

    public LevelTwoHandler(IMeridianClient client)
    {
        _client = client;
    }

    public Task<RequestPayload?> ProcessAsync(RequestPayload? layerOneResult = null)
    {
        return null;
    }
}
