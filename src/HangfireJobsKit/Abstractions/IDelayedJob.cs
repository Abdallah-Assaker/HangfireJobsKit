namespace HangfireJobsKit.Abstractions;

/// <summary>
/// Interface for jobs that need to be executed once, either immediately or after a specified delay
/// </summary>
public interface IDelayedJob : IJob { }