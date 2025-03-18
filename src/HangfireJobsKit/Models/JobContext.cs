using Newtonsoft.Json;

namespace HangfireJobsKit.Models;

/// <summary>
/// Holds contextual information for job execution
/// </summary>
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public record JobContext(
    [property: JsonProperty] string CorrelationId,
    [property: JsonProperty] IDictionary<string, string> Headers
)
{
    [JsonConstructor]
    public JobContext(string correlationId) 
        : this(correlationId, new Dictionary<string, string>())
    {
    }
}