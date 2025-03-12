using System.Diagnostics;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Extensions;
using HangfireJobsKit.Models;

namespace HangfireJobsKit.SampleApp.Filters;

public class AuditLogFilter : IHangfireJobFilter
{
    private readonly ILogger<AuditLogFilter> _logger;
    private readonly Dictionary<string, Stopwatch> _jobTimers = new();
    
    public AuditLogFilter(ILogger<AuditLogFilter> logger)
    {
        _logger = logger;
    }
    
    public double ExecutionOrder => 10; // Run after transaction filters
    
    public void OnCreating(CreatingContext context) { }
    
    public void OnCreated(CreatedContext context) { }
    
    public void OnPerforming(PerformingContext context)
    {
        var jobId = context.BackgroundJob.Id;
        var jobContext = context.GetJobContext();
        
        _logger.LogInformation(
            "Job {JobId} starting. Correlation: {CorrelationId}",
            jobId,
            jobContext?.CorrelationId ?? "none");
            
        var timer = new Stopwatch();
        timer.Start();
        _jobTimers[jobId] = timer;
    }
    
    public void OnPerformed(PerformedContext context)
    {
        var jobId = context.BackgroundJob.Id;
        
        if (_jobTimers.TryGetValue(jobId, out var timer))
        {
            timer.Stop();
            var jobContext = context.GetJobContext();
            
            _logger.LogInformation(
                "Job {JobId} completed in {ElapsedMs}ms. Success: {Success}, Correlation: {CorrelationId}",
                jobId,
                timer.ElapsedMilliseconds,
                context.Exception == null,
                jobContext?.CorrelationId ?? "none");
                
            _jobTimers.Remove(jobId);
        }
    }
    
    public void OnStateElection(ElectStateContext context) { }
    
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
    
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
}