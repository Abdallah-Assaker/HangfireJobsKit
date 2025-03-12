namespace HangfireJobsKit.Models;

/// <summary>
/// Defines what should happen when retry attempts are exceeded
/// </summary>
public enum AttemptsExceededAction
{
    /// <summary>
    /// Job remains in failed state
    /// </summary>
    Fail,
    
    /// <summary>
    /// Job is deleted from the queue
    /// </summary>
    Delete
}
