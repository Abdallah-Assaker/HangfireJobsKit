using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Models;

namespace HangfireJobsKit.Jobs;

/// <summary>
/// Combined manager for all job types using Hangfire
/// </summary>
public class HangfireJobManager : IJobManager
{
    private readonly IDelayedJobManager _delayedJobManager;
    private readonly IRecurrenceJobManager _recurrenceJobManager;

    /// <summary>
    /// Creates a new instance of HangfireJobManager
    /// </summary>
    /// <param name="delayedJobManager">The delayed job manager</param>
    /// <param name="recurrenceJobManager">The recurrence job manager</param>
    public HangfireJobManager(
        IDelayedJobManager delayedJobManager, 
        IRecurrenceJobManager recurrenceJobManager)
    {
        _delayedJobManager = delayedJobManager;
        _recurrenceJobManager = recurrenceJobManager;
    }

    /// <summary>
    /// Schedules a job to be executed after a specified delay
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="job">The job to schedule</param>
    /// <param name="delayedMilliseconds">Delay in milliseconds before executing the job</param>
    /// <param name="queue">The queue to place the job in</param>
    /// <param name="context">The job context</param>
    public void Schedule<TJob>(TJob job, int delayedMilliseconds = 0, string? queue = default, JobContext? context = default) 
        where TJob : IDelayedJob
    {
        _delayedJobManager.Schedule(job, delayedMilliseconds, queue, context);
    }

    /// <summary>
    /// Enqueues a job for immediate execution
    /// </summary>
    /// <typeparam name="TJob">Type of job to enqueue</typeparam>
    /// <param name="job">The job to enqueue</param>
    /// <param name="queue">The queue to place the job in</param>
    /// <param name="context">The job context</param>
    public void Enqueue<TJob>(TJob job, string? queue = default, JobContext? context = default) 
        where TJob : IDelayedJob
    {
        _delayedJobManager.Enqueue(job, queue, context);
    }

    /// <summary>
    /// Adds or updates a recurring job
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="jobId">Unique identifier for the recurring job</param>
    /// <param name="job">The job to execute on schedule</param>
    /// <param name="cron">Cron expression defining the schedule</param>
    /// <param name="queue">The queue to place the job in</param>
    /// <param name="context">The job context</param>
    public void AddOrUpdateRecurring<TJob>(string jobId,
        TJob job,
        string cron,
        string? queue = default,
        JobContext? context = null) where TJob : IRecurrenceJob
    {
        _recurrenceJobManager.AddOrUpdateRecurring(jobId, job, cron, queue, context);
    }
}