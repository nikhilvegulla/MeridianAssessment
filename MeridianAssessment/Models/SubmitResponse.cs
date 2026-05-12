using System.Text.Json.Serialization;

namespace MeridianAssessment.Models;

/// <summary>JSON body from <c>POST /api/v1/submit</c> (success or graded attempt).</summary>
public class SubmitResponse
{
    public bool Correct { get; set; }

    public int Layer { get; set; }

    public string? Message { get; set; }

    [JsonPropertyName("submission_id")]
    public string? SubmissionId { get; set; }

    public string? Type { get; set; }
}
