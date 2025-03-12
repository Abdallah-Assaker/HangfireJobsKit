using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Hangfire;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Extensions;
using HangfireJobsKit.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Jobs;

/// <summary>
/// Manager for delayed jobs using Hangfire
/// </summary>
public class HangfireDelayedJobManager : IDelayedJobManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundJobClient _backgroundJobManager;
    private readonly ILogger<HangfireDelayedJobManager> _logger;

    /// <summary>
    /// Creates a new instance of HangfireDelayedJobManager
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="backgroundJobManager">The Hangfire background job client</param>
    /// <param name="logger">The logger</param>
    public HangfireDelayedJobManager(
        IServiceProvider serviceProvider,
        IBackgroundJobClient backgroundJobManager,
        ILogger<HangfireDelayedJobManager> logger)
    {
        _serviceProvider = serviceProvider;
        _backgroundJobManager = backgroundJobManager;
        _logger = logger;
    }

    /// <summary>
    /// Schedules a job to be executed after a specified delay
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="job">The job to schedule</param>
    /// <param name="context">The job context</param>
    /// <param name="delayedMilliseconds">Delay in milliseconds before executing the job</param>
    /// <param name="queue">The queue to place the job in</param>
    public void Schedule<TJob>(
        [Required] TJob job, 
        JobContext context, 
        int delayedMilliseconds = 0, 
        string queue = "default"
    ) where TJob : IDelayedJob
    {
        var configuredDisplayName = job.GetJobConfigurationDisplayName();
        var configuredQueue = job.GetJobConfigurationQueue();
        
        // Use configured queue if the caller didn't specify one explicitly
        if (queue == "default" && configuredQueue != "default")
        {
            queue = configuredQueue;
        }
        
        var jobId = _backgroundJobManager.Schedule(
            queue,
            () => ExecuteJob(job, context, configuredDisplayName),
            TimeSpan.FromMilliseconds(delayedMilliseconds)
        );
        
        _logger.LogDebug("Scheduled job {JobType} with ID {JobId} and delay {Delay}ms in queue {Queue}", 
            typeof(TJob).Name, jobId, delayedMilliseconds, queue);
    }

    /// <summary>
    /// Enqueues a job for immediate execution
    /// </summary>
    /// <typeparam name="TJob">Type of job to enqueue</typeparam>
    /// <param name="job">The job to enqueue</param>
    /// <param name="context">The job context</param>
    /// <param name="queue">The queue to place the job in</param>
    public void Enqueue<TJob>(
        [Required] TJob job, 
        JobContext context, 
        string queue = "default"
    ) where TJob : IDelayedJob
    {
        var configuredDisplayName = job.GetJobConfigurationDisplayName();
        var configuredQueue = job.GetJobConfigurationQueue();
        
        // Use configured queue if the caller didn't specify one explicitly
        if (queue == "default" && configuredQueue != "default")
        {
            queue = configuredQueue;
        }
        
        var jobId = _backgroundJobManager.Enqueue(
            queue,
            () => ExecuteJob(job, context, configuredDisplayName)
        );
        
        _logger.LogDebug("Enqueued job {JobType} with ID {JobId} in queue {Queue}", 
            typeof(TJob).Name, jobId, queue);
    }

    /// <summary>
    /// Executes the job using the appropriate handler
    /// </summary>
    /// <typeparam name="TJob">Type of job to execute</typeparam>
    /// <param name="job">The job to execute</param>
    /// <param name="context">The job context</param>
    /// <param name="jobName">The job display name</param>
    /// <returns>The execution task</returns>
    [DisplayName("{2}")]
    public Task ExecuteJob<TJob>(TJob job, JobContext context, string jobName) where TJob : IDelayedJob
    {
        var handler = _serviceProvider.GetRequiredService<IDelayedJobHandlerBase<TJob>>();
        return handler.Execute(job, context);
    }
}