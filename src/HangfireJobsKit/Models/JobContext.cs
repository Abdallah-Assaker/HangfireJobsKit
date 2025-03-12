using System.Text.Json.Serialization;

namespace HangfireJobsKit.Models;

/// <summary>
/// Holds contextual information for job execution
/// </summary>
[JsonSerializable(typeof(JobContext))]
public record JobContext(
    string CorrelationId,
    IDictionary<string, string> Headers
)
{
    [JsonConstructor]
    public JobContext(string correlationId) 
        : this(correlationId, new Dictionary<string, string>())
    {
    }
}