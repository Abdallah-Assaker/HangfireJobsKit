namespace HangfireJobsKit.Abstractions.Handlers;

/// <summary>
/// Interface for handlers that process recurrence jobs
/// </summary>
/// <typeparam name="TJob">Type of recurrence job this handler can process</typeparam>
public interface IRecurrenceJobHandlerBase<in TJob> : IBackgroundJobHandlerBase<TJob> 
    where TJob : IRecurrenceJob 
{ }