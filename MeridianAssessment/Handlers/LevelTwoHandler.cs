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

    /// <summary>
    /// Decrypts (or proves decryption of) the sample dataset and submits the layer-2 payload.
    /// </summary>
    /// <param name="layerOneResult">Optional outcome from layer 1 for sequencing in <see cref="Program"/>.</param>
    public Task<RequestPayload?> ProcessAsync(RequestPayload? layerOneResult = null)
    {
        
    }
}
