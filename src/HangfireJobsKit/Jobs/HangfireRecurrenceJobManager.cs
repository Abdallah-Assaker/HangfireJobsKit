using System.ComponentModel;
using Hangfire;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Extensions;
using HangfireJobsKit.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Jobs;

/// <summary>
/// Manager for recurrence jobs using Hangfire
/// </summary>
internal class HangfireRecurrenceJobManager : IRecurrenceJobManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly ILogger<HangfireRecurrenceJobManager> _logger;

    /// <summary>
    /// Creates a new instance of HangfireRecurrenceJobManager
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="recurringJobManager">The Hangfire recurring job manager</param>
    /// <param name="logger">The logger</param>
    public HangfireRecurrenceJobManager(
        IServiceProvider serviceProvider,
        IRecurringJobManager recurringJobManager,
        ILogger<HangfireRecurrenceJobManager> logger)
    {
        _serviceProvider = serviceProvider;
        _recurringJobManager = recurringJobManager;
        _logger = logger;
    }

    /// <summary>
    /// Adds or updates a recurring job
    /// </summary>
    /// <typeparam name="TJob">Type of job to schedule</typeparam>
    /// <param name="jobId">Unique identifier for the recurring job</param>
    /// <param name="job">The job to execute on schedule</param>
    /// <param name="context">The job context</param>
    /// <param name="cron">Cron expression defining the schedule</param>
    /// <param name="queue">The queue to place the job in</param>
    public void AddOrUpdateRecurring<TJob>(
        string jobId, 
        TJob job, 
        JobContext context,
        string cron,
        string queue = "default"
    ) where TJob : IRecurrenceJob
    {
        var configuredDisplayName = job.GetJobConfigurationDisplayName();
        var configuredQueue = job.GetJobConfigurationQueue();
        
        // Use configured queue if the caller didn't specify one explicitly
        if (queue == "default" && configuredQueue != "default")
        {
            queue = configuredQueue;
        }
        
        _recurringJobManager.AddOrUpdate(
            jobId,
            queue,
            () => ExecuteJob(job, context, configuredDisplayName),
            cron,
            new RecurringJobOptions 
            {
                TimeZone = TimeZoneInfo.Utc,
                MisfireHandling = MisfireHandlingMode.Relaxed
            }
        );
        
        _logger.LogDebug("Updated recurring job {JobId} of type {JobType} with schedule {Cron} in queue {Queue}", 
            jobId, typeof(TJob).Name, cron, queue);
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
    public Task ExecuteJob<TJob>(TJob job, JobContext context, string jobName) where TJob : IRecurrenceJob
    {
        var handler = _serviceProvider.GetRequiredService<IRecurrenceJobHandlerBase<TJob>>();
        return handler.Execute(job, context);
    }
}