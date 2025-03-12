using HangfireJobsKit.Models;

namespace HangfireJobsKit.Configuration;

/// <summary>
/// Attribute for configuring job execution behavior
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class JobConfigurationAttribute : Attribute
{
    /// <summary>
    /// Display name for the job in Hangfire dashboard
    /// </summary>
    public string? DisplayName { get; }
    
    /// <summary>
    /// Number of retry attempts for failed jobs
    /// </summary>
    public int RetryAttempts { get; }
    
    /// <summary>
    /// Delay between retry attempts in seconds
    /// </summary>
    public int[] RetryDelaysInSeconds { get; }
    
    /// <summary>
    /// Action to take when retry attempts are exceeded
    /// </summary>
    public AttemptsExceededAction OnAttemptsExceeded { get; }
    
    /// <summary>
    /// Exception types that should not trigger retries
    /// </summary>
    public Type[] ExceptOn { get; }
    
    /// <summary>
    /// Queue name for the job
    /// </summary>
    public string Queue { get; }
    
    /// <summary>
    /// Indicates whether job execution events should be logged
    /// </summary>
    public bool LogEvents { get; }

    /// <summary>
    /// Creates a new instance of JobConfigurationAttribute
    /// </summary>
    /// <param name="displayName">Display name for the job</param>
    /// <param name="retryAttempts">Number of retry attempts for failed jobs</param>
    /// <param name="onAttemptsExceeded">Action to take when retry attempts are exceeded</param>
    /// <param name="retryDelaysInSeconds">Delay between retry attempts in seconds</param>
    /// <param name="exceptOn">Exception types that should not trigger retries</param>
    /// <param name="queue">Queue name for the job</param>
    /// <param name="logEvents">Whether to log job execution events</param>
    public JobConfigurationAttribute(
        string? displayName = null,
        int retryAttempts = 3,
        AttemptsExceededAction onAttemptsExceeded = AttemptsExceededAction.Fail,
        int[]? retryDelaysInSeconds = null,
        Type[]? exceptOn = null,
        string queue = "default",
        bool logEvents = false)
    {
        DisplayName = displayName;
        RetryAttempts = retryAttempts;
        RetryDelaysInSeconds = retryDelaysInSeconds ?? [1];
        OnAttemptsExceeded = onAttemptsExceeded;
        ExceptOn = exceptOn ?? [];
        Queue = queue;
        LogEvents = logEvents;
    }
}