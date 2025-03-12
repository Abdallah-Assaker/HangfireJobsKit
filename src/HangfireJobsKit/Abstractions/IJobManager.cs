namespace HangfireJobsKit.Abstractions;

/// <summary>
/// Combined interface for all job management operations
/// </summary>
public interface IJobManager : IRecurrenceJobManager, IDelayedJobManager { }