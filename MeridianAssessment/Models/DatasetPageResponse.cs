using System.Text.Json.Serialization;

namespace MeridianAssessment.Models;

/// <summary>
/// Paginated dataset envelope from the sample GET (Base64 ciphertext rows in <see cref="Data"/>).
/// </summary>
public class DatasetPageResponse
{
    public List<string> Data { get; set; } = [];

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    public int Page { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    public int Total { get; set; }
}
