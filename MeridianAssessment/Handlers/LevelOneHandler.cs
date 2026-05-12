using MeridianAssessment.Constants;
using MeridianAssessment.Interfaces;
using MeridianAssessment.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MeridianAssessment.Handlers;

public class LevelOneHandler
{
    private static readonly JsonSerializerOptions WriteJsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static readonly JsonSerializerOptions CanonicalDatasetJson = new()
    {
        WriteIndented = false,
    };

    private static string SampleDataSetCachePath =>
        Path.Combine(AppContext.BaseDirectory, FilePathConstants.SampleDataSetFilePath);

    private readonly IMeridianClient _client;

    public object? ResponseData { get; set; }

    public LevelOneHandler(IMeridianClient client)
    {
        _client = client;
    }

    public async Task<SubmitResponse?> ProcessAsync()
    {
        var dataSet = await GetDataSetAsync();
        if (dataSet.Count == 0)
        {
            Console.WriteLine("No data set found");
            return null;
        }

        var result = await SubmitContentHashAsync(dataSet);
        ResponseData = result;
        return result;
    }

    private async Task<List<string>> GetDataSetAsync()
    {
        var existing = await ReadDataSetAsync();
        if (existing.Count > 0)
            return existing;

        var hasMore = true;
        var allRows = new List<string>();
        var pageNumber = 1;

        while (hasMore)
        {
            var response = await _client.GetSampleDataSetAsync(pageNumber);
            allRows.AddRange(response?.Data ?? []);
            hasMore = response?.HasMore ?? false;
            pageNumber++;
        }

        await WriteDataSetAsync(allRows);
        return allRows;
    }

    private async Task WriteDataSetAsync(List<string> dataSet)
    {
        var dataSetJson = JsonSerializer.Serialize(dataSet, WriteJsonOptions);
        await File.WriteAllTextAsync(SampleDataSetCachePath, dataSetJson);
    }

    private async Task<List<string>> ReadDataSetAsync()
    {
        if (!File.Exists(SampleDataSetCachePath))
            return [];

        try
        {
            var dataSetJson = await File.ReadAllTextAsync(SampleDataSetCachePath);
            if (string.IsNullOrWhiteSpace(dataSetJson))
                return [];

            return JsonSerializer.Deserialize<List<string>>(dataSetJson, WriteJsonOptions) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }

    //private async Task<SubmitResponse?> SubmitContentHashAsync(List<string> dataSet)
    //{
    //    //var canonical = JsonSerializer.Serialize(dataSet, CanonicalDatasetJson);
    //    //var utf8 = Encoding.UTF8.GetBytes(canonical);
    //    //var hashHex = Convert.ToHexString(SHA256.HashData(utf8)).ToLowerInvariant();

    //    //dataSet.Sort(StringComparer.Ordinal);
    //    string joined = string.Join("\n", dataSet);

    //    // 3. JOINING
    //    // Usually, "byte-level integrity" means hashing the strings joined directly.
    //    string combined = string.Join("\n",dataSet);

    //    // 4. HASHING
    //    byte[] bytes = Encoding.UTF8.GetBytes(combined);
    //    byte[] hash = SHA256.HashData(bytes);
    //    string hashHex = Convert.ToHexString(hash).ToLowerInvariant();

    //    var payload = new RequestPayload
    //    {
    //        Type = LayerTypeConstants.ContentHash,
    //        Value = hashHex,
    //        Notes = $"sha256-lower-hex;utf8-minified-json-array;rows={dataSet.Count}",
    //    };
    //    return await _client.SubmitTask(payload);
    //}

    private async Task<SubmitResponse?> SubmitContentHashAsync(List<string> dataSet)
    {

        // Create minified JSON array
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };

        string canonical = JsonSerializer.Serialize(dataSet, options);

        // UTF8 bytes
        byte[] bytes = Encoding.UTF8.GetBytes(canonical);

        // SHA256 hash
        byte[] hash = SHA256.HashData(bytes);

        // lowercase hex
        string hashHex = Convert.ToHexString(hash).ToUpperInvariant();

        var payload = new RequestPayload
        {
            Type = LayerTypeConstants.ContentHash,
            Value = hashHex,
            Notes = $"sha256-lower-hex;utf8-minified-json-array;rows={dataSet.Count}",
        };
        Console.WriteLine($"Submitting content hash: {dataSet.Distinct().Count()}");
        return await _client.SubmitTask(payload);
    }
}
