namespace HangfireJobsKit.Abstractions.Handlers;

/// <summary>
/// Interface for handlers that process delayed jobs
/// </summary>
/// <typeparam name="TJob">Type of delayed job this handler can process</typeparam>
public interface IDelayedJobHandlerBase<in TJob> : IBackgroundJobHandlerBase<TJob> 
    where TJob : IDelayedJob 
{ }