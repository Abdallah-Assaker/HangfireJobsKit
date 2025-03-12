namespace HangfireJobsKit.Filters.Core;

using System.Reflection;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;
using HangfireJobsKit.Models;

/// <summary>
/// Filter that extracts job configuration attributes and applies them to the job
/// </summary>
public class JobConfigurationFilter : IClientFilter, IServerFilter, IElectStateFilter
{
    /// <summary>
    /// Called when a background job is creating
    /// </summary>
    public void OnCreating(CreatingContext context)
    {
        var jobArg = context.Job.Args.FirstOrDefault(arg => arg is IJob);
        if (jobArg is not IJob job) return;
        
        var jobConfig = job.GetType().GetCustomAttribute<JobConfigurationAttribute>();
        if (jobConfig == null) return;
        
        context.SetJobParameter("DisplayName", jobConfig.DisplayName ?? job.GetType().Name);
        context.SetJobParameter("RetryAttempts", jobConfig.RetryAttempts);
        context.SetJobParameter("RetryDelaysInSeconds", jobConfig.RetryDelaysInSeconds);
        context.SetJobParameter("OnAttemptsExceeded", jobConfig.OnAttemptsExceeded);
        context.SetJobParameter("LogEvents", jobConfig.LogEvents);
        
        var exceptOnTypeNames = jobConfig.ExceptOn.Select(t => t.FullName).ToArray();
        context.SetJobParameter("ExceptOn", exceptOnTypeNames);
    }

    /// <summary>
    /// Called when a background job has been created
    /// </summary>
    public void OnCreated(CreatedContext context) { }

    /// <summary>
    /// Called before the execution of a background job
    /// </summary>
    public void OnPerforming(PerformingContext context) { }

    /// <summary>
    /// Called after the execution of a background job
    /// </summary>
    public void OnPerformed(PerformedContext context) { }

    /// <summary>
    /// Called during state election for a job
    /// </summary>
    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is FailedState failedState)
        {
            var exceptOn = context.GetJobParameter<string[]>("ExceptOn") ?? Array.Empty<string>();
            var exceptionTypeName = failedState.Exception.GetType().FullName;
            
            // If this exception type is in the ExceptOn list, we don't retry
            if (exceptOn.Contains(exceptionTypeName))
            {
                context.CandidateState = new FailedState(failedState.Exception)
                {
                    Reason = "Exception type is in ExceptOn list"
                };
                return;
            }
            
            var retryAttempts = context.GetJobParameter<int?>("RetryAttempts") ?? 3;
            var retryDelays = context.GetJobParameter<int[]>("RetryDelaysInSeconds") ?? new[] { 1 };
            var exceededAction = context.GetJobParameter<AttemptsExceededAction?>("OnAttemptsExceeded") ?? AttemptsExceededAction.Fail;
            var currentAttempt = context.GetJobParameter<int?>("RetryCount") ?? 0;
            
            if (currentAttempt < retryAttempts)
            {
                var delayInSeconds = retryDelays.Length > currentAttempt 
                    ? retryDelays[currentAttempt] 
                    : retryDelays.LastOrDefault();
                    
                var scheduledState = new ScheduledState(TimeSpan.FromSeconds(delayInSeconds))
                {
                    Reason = $"Retry attempt {currentAttempt + 1} of {retryAttempts}"
                };
                
                context.SetJobParameter("RetryCount", currentAttempt + 1);
                context.CandidateState = scheduledState;
            }
            else if (exceededAction == AttemptsExceededAction.Delete)
            {
                context.CandidateState = new DeletedState
                {
                    Reason = "Retry attempts exceeded"
                };
            }
        }
    }
}