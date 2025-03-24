using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using HangfireJobsKit.Models;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Handlers;

/// <summary>
/// Base implementation for all job handlers
/// </summary>
/// <typeparam name="TJob">Type of job this handler can process</typeparam>
public abstract class BackgroundJobHandlerBase<TJob> : IBackgroundJobHandlerBase<TJob> 
    where TJob : IJob
{
    /// <summary>
    /// Executes the job with the given context
    /// </summary>
    /// <param name="job">The job to execute</param>
    /// <param name="context">The execution context</param>
    public async Task Execute(TJob job, JobContext? context)
    {
        await Handle(job);
    }

    /// <summary>
    /// Handles the job execution logic
    /// </summary>
    /// <param name="job">The job to handle</param>
    protected abstract Task Handle(TJob job);
}