using System.ComponentModel;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Extensions;
using HangfireJobsKit.Models;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Filters.DefaultFilters;

/// <summary>
/// Filter that logs job execution errors
/// </summary>
public class ErrorLoggingFilter : IHangfireJobFilter
{
    private readonly ILogger<ErrorLoggingFilter> _logger;

    /// <summary>
    /// Creates a new instance of ErrorLoggingFilter
    /// </summary>
    /// <param name="logger">The logger</param>
    public ErrorLoggingFilter(ILogger<ErrorLoggingFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the execution order for this filter
    /// </summary>
    public double ExecutionOrder => 1;

    /// <summary>
    /// Called when a background job is creating
    /// </summary>
    public void OnCreating(CreatingContext context) { }

    /// <summary>
    /// Called when a background job has been created
    /// </summary>
    public void OnCreated(CreatedContext context) { }

    /// <summary>
    /// Called before the execution of a background job
    /// </summary>
    public void OnPerforming(PerformingContext context) 
    {
        if (context.GetJobParameter<bool>("LogEvents"))
        {
            var contextData = context.GetJobContext();
            _logger.LogInformation(
                "Starting job {JobType} execution. Correlation: {CorrelationId}",
                context.BackgroundJob.Job.Type.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                    .FirstOrDefault() is DisplayNameAttribute attr
                    ? attr.DisplayName
                    : context.BackgroundJob.Job.Type.Name,
                contextData?.CorrelationId ?? "none");
        }
    }

    /// <summary>
    /// Called after the execution of a background job
    /// </summary>
    public void OnPerformed(PerformedContext context)
    {
        if (context.Exception is null) 
        {
            if (context.GetJobParameter<bool>("LogEvents"))
            {
                var contextData = context.GetJobContext();
                _logger.LogInformation(
                    "Completed job {JobType} execution. Correlation: {CorrelationId}",
                    context.BackgroundJob.Job.Type.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                        .FirstOrDefault() is DisplayNameAttribute attr
                        ? attr.DisplayName
                        : context.BackgroundJob.Job.Type.Name,
                    contextData?.CorrelationId ?? "none");
            }
            return;
        }

        var job = context.BackgroundJob.Job;
        var jobContextData = context.GetJobContext();
        
        _logger.LogError(context.Exception,
            "Job {JobType} execution failed. Correlation: {CorrelationId}, Args: {Args}",
            job.Type.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                .FirstOrDefault() is DisplayNameAttribute nameAttr
                ? nameAttr.DisplayName
                : job.Type.Name,
            
            jobContextData?.CorrelationId ?? "none",
            string.Join(", ", job.Args.Where(a => a is not JobContext)));
    }

    /// <summary>
    /// Called during state election for a job
    /// </summary>
    public void OnStateElection(ElectStateContext context) { }

    /// <summary>
    /// Called after a state was successfully applied
    /// </summary>
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }

    /// <summary>
    /// Called when a state application failed
    /// </summary>
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
}
