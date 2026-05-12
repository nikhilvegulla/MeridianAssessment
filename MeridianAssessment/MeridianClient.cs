using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;
using System.Text;
using System.Text.Json;

namespace MeridianAssessment;

public class MeridianClient: IMeridianClient
{
    private HttpClient httpClient;
    private string baseUrl = "";
    private string apiKey = "";


    public MeridianClient()
    {
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<object> GetSampleDataSet()
    {
        var fileContent = FileHelper.ReadFromFile(Constants.FilePathConstants.SampleDataSetFilePath);
        if(!string.IsNullOrEmpty(fileContent))
        {
            return fileContent;
        }
        var response = await ExecuteWithRetry(async () => await httpClient.GetAsync($"{baseUrl}"));
        var data = await response.Content.ReadAsStringAsync();
        if(!string.IsNullOrEmpty(data))
        {
            FileHelper.WriteToFile(Constants.FilePathConstants.SampleDataSetFilePath, data);
            return JsonSerializer.Deserialize<object>(data);
        }
        return null;
    }

    public async Task<RequestPayload> SubmitTask(RequestPayload payload)
    {
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        var response = await ExecuteWithRetry(async () => await httpClient.PostAsync($"{baseUrl}/submit", content));
        var responseContent = await response.Content.ReadAsStringAsync();
        if(!string.IsNullOrEmpty(responseContent))
        {
            return JsonSerializer.Deserialize<RequestPayload>(responseContent);
        }
        return null;
        return result;
    }

    public async Task<string> GetSecretKey()
    {
        var response = await ExecuteWithRetry(async () => await httpClient.GetAsync($"{baseUrl}/secret"));
        var apiKey = await response.Content.ReadAsStringAsync();
        if(!string.IsNullOrEmpty(apiKey))
        {
            return JsonSerializer.Deserialize<string>(apiKey);
        }
        return null;
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action)
    {
        try {
            return await action();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests) {
            Console.WriteLine("Rate limit hit. Checking headers...");
            var retryAfter = ex.Headers.GetValues("Retry-After").FirstOrDefault();
            await Task.Delay(int.Parse(retryAfter)); 
            return await ExecuteWithRetry(action);
        }
    }
}
