using HangfireJobsKit.Models;

namespace HangfireJobsKit.Abstractions;

/// <summary>
/// Interface for managing delayed jobs
/// </summary>
public interface IDelayedJobManager
{
    /// <summary>
    /// Schedules a job to be executed after a specified delay
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="job">The job to schedule</param>
    /// <param name="delayedMilliseconds">Delay in milliseconds before executing the job</param>
    /// <param name="queue">The queue to place the job in</param>
    /// <param name="context">The job context</param>
    void Schedule<TJob>(TJob job, int delayedMilliseconds = 0, string? queue = default, JobContext? context = null) 
        where TJob : IDelayedJob;
    
    /// <summary>
    /// Enqueues a job for immediate execution
    /// </summary>
    /// <typeparam name="TJob">Type of job to enqueue</typeparam>
    /// <param name="job">The job to enqueue</param>
    /// <param name="queue">The queue to place the job in</param>
    /// <param name="context">The job context</param>
    void Enqueue<TJob>(TJob job, string? queue = default, JobContext? context = default)
        where TJob : IDelayedJob;
}