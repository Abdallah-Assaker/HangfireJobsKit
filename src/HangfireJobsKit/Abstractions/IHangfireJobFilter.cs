using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace HangfireJobsKit.Abstractions;

/// <summary>
/// Interface for creating custom Hangfire job filters
/// </summary>
public interface IHangfireJobFilter
{
    /// <summary>
    /// Determines the order of filter execution (ascending order)
    /// </summary>
    double ExecutionOrder { get; }
    
    /// <summary>
    /// Called when a background job is creating
    /// </summary>
    void OnCreating(CreatingContext context);
    
    /// <summary>
    /// Called when a background job has been created
    /// </summary>
    void OnCreated(CreatedContext context);
    
    /// <summary>
    /// Called before the execution of a background job
    /// </summary>
    void OnPerforming(PerformingContext context);
    
    /// <summary>
    /// Called after the execution of a background job
    /// </summary>
    void OnPerformed(PerformedContext context);
    
    /// <summary>
    /// Called during state election for a job
    /// </summary>
    void OnStateElection(ElectStateContext context);
    
    /// <summary>
    /// Called after a state was successfully applied
    /// </summary>
    void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction);
    
    /// <summary>
    /// Called when a state application failed
    /// </summary>
    void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction);
}