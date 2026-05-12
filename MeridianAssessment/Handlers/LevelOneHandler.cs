using MeridianAssessment.Enums;
using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;

namespace MeridianAssessment.Handlers;

/// <summary>
/// Level 1: load the dataset as exact response bytes, then submit with Base64(raw bytes) in <see cref="RequestPayload.Value"/>
/// so the server can verify byte-level integrity without JSON re-serialization changing the payload.
/// </summary>
public class LevelOneHandler
{
    private readonly IMeridianClient _client;

    public object? ResponseData { get; set; }

    public LevelOneHandler(IMeridianClient client)
    {
        _client = client;
    }

    public async Task<RequestPayload?> ProcessAsync()
    {
        
    }
}
