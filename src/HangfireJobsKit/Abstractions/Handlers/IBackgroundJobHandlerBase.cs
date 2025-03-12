using HangfireJobsKit.Models;

namespace HangfireJobsKit.Abstractions.Handlers;

/// <summary>
/// Base interface for all job handlers
/// </summary>
/// <typeparam name="TJob">Type of job this handler can process</typeparam>
public interface IBackgroundJobHandlerBase<in TJob> where TJob : IJob
{
    /// <summary>
    /// Executes the job with the given context
    /// </summary>
    /// <param name="job">The job to execute</param>
    /// <param name="context">The execution context</param>
    Task Execute(TJob job, JobContext context);
}
