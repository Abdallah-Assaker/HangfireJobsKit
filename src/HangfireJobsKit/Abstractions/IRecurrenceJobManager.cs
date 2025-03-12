using HangfireJobsKit.Models;

namespace HangfireJobsKit.Abstractions;

/// <summary>
/// Interface for managing recurring jobs
/// </summary>
public interface IRecurrenceJobManager
{
    /// <summary>
    /// Adds or updates a recurring job
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="jobId">Unique identifier for the recurring job</param>
    /// <param name="job">The job to execute on schedule</param>
    /// <param name="context">The job context</param>
    /// <param name="cron">Cron expression defining the schedule</param>
    /// <param name="queue">The queue to place the job in</param>
    void AddOrUpdateRecurring<TJob>(
        string jobId, 
        TJob job, 
        JobContext context,
        string cron,
        string queue = "default"
    ) where TJob : IRecurrenceJob;
}