using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MeridianAssessment;

public class MeridianClient : IMeridianClient
{
    public static readonly JsonSerializerOptions ApiJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private const int MaxRateLimitRetryAttempts = 5;

    private readonly HttpClient httpClient;
    private string baseUrl = "https://ca-seassessment-api-dev.happywater-190f264d.northcentralus.azurecontainerapps.io";
    private string apiKey = "sa_2f8d50eb4593576221970f4573c4e3951877dce400e4538e289425613eae5f14";

    public MeridianClient()
    {
        httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(10);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<DatasetPageResponse?> GetSampleDataSetAsync(int pageNumber = 1, int pageSize = 100)
    {
        var raw = await GetSampleDataSetPageBytesAsync(pageNumber, pageSize);
        if (raw is not { Length: > 0 })
            return null;
        return JsonSerializer.Deserialize<DatasetPageResponse>(raw, ApiJsonOptions);
    }

    public async Task<byte[]?> GetSampleDataSetPageBytesAsync(int pageNumber = 1, int pageSize = 100)
    {
        var response = await SendWithRetryAsync(() =>
            httpClient.GetAsync($"{baseUrl}/api/v1/dataset?page={pageNumber}&page_size={pageSize}"));
        try
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
        finally
        {
            response.Dispose();
        }
    }

    public async Task<SubmitResponse?> SubmitTask(RequestPayload payload)
    {
        var response = await SendWithRetryAsync(async () =>
        {
            var payloadJson = JsonSerializer.Serialize(payload, ApiJsonOptions);
            using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync($"{baseUrl}/api/v1/submit", content);
        });
        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseContent))
                return JsonSerializer.Deserialize<SubmitResponse>(responseContent, ApiJsonOptions);
            return null;
        }
        finally
        {
            response.Dispose();
        }
    }

    public async Task<string?> GetSecretKey()
    {
        var response = await SendWithRetryAsync(() => httpClient.GetAsync($"{baseUrl}/secret"));
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(body))
                return JsonSerializer.Deserialize<string>(body, ApiJsonOptions);
            return null;
        }
        finally
        {
            response.Dispose();
        }
    }

    /// <summary>Retries on 429 using <c>Retry-After</c> (at most <see cref="MaxRateLimitRetryAttempts"/> waits); otherwise enforces success status.</summary>
    private async Task<HttpResponseMessage> SendWithRetryAsync(Func<Task<HttpResponseMessage>> send)
    {
        var rateLimitRetries = 0;
        while (true)
        {
            var response = await send();
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (rateLimitRetries >= MaxRateLimitRetryAttempts)
                {
                    response.Dispose();
                    throw new HttpRequestException(
                        $"Rate limited: exceeded maximum retry attempts ({MaxRateLimitRetryAttempts}).",
                        null,
                        HttpStatusCode.TooManyRequests);
                }

                rateLimitRetries++;
                Console.WriteLine($"Rate limit hit ({rateLimitRetries}/{MaxRateLimitRetryAttempts}). Waiting...");
                var delay = GetRetryAfterDelay(response);
                response.Dispose();
                await Task.Delay(delay);
                continue;
            }

            return response.EnsureSuccessStatusCode();
        }
    }

    private static TimeSpan GetRetryAfterDelay(HttpResponseMessage response)
    {
        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter?.Delta is { TotalMilliseconds: > 0 } d)
            return d;
        if (retryAfter?.Date is { } until)
        {
            var wait = until - DateTimeOffset.UtcNow;
            if (wait > TimeSpan.Zero)
                return wait > TimeSpan.FromHours(1) ? TimeSpan.FromSeconds(5) : wait;
        }

        return TimeSpan.FromSeconds(5);
    }
}
