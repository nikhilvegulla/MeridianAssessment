using MeridianAssessment.Models;

namespace MeridianAssessment.Interfaces;

public interface IMeridianClient
{
    /// <summary>Full sample dataset as exact HTTP body bytes (cached on disk).</summary>
    Task<byte[]?> GetSampleDataSetAsync();

    Task<RequestPayload?> SubmitTask(RequestPayload payload);

    Task<string?> GetSecretKey();
}
