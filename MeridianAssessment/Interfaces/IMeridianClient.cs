using MeridianAssessment.Models;

namespace MeridianAssessment.Interfaces;

public interface IMeridianClient
{
    Task<DatasetPageResponse?> GetSampleDataSetAsync(int pageNumber = 1, int pageSize = 100);

    /// <summary>Exact UTF-8 body bytes for one dataset page (for <c>content_hash</c> over wire content).</summary>
    Task<byte[]?> GetSampleDataSetPageBytesAsync(int pageNumber = 1, int pageSize = 100);

    Task<SubmitResponse?> SubmitTask(RequestPayload payload);

    Task<string?> GetSecretKey();
}
