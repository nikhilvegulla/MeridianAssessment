using MeridianAssessment.Models;

namespace MeridianAssessment.Interfaces;

public interface IMeridianClient
{
    public Task<object> GetSampleDataSet();

    public Task<RequestPayload> SubmitTask(RequestPayload payload);

    public Task<string> GetSecretKey();
}
