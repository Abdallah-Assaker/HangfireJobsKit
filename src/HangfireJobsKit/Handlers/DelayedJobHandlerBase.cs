using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Handlers;

/// <summary>
/// Base implementation for delayed job handlers
/// </summary>
/// <typeparam name="TJob">Type of delayed job this handler can process</typeparam>
public abstract class DelayedJobHandlerBase<TJob> : BackgroundJobHandlerBase<TJob>, IDelayedJobHandlerBase<TJob>
    where TJob : IDelayedJob
{
    protected DelayedJobHandlerBase(ILogger logger) : base(logger)
    {
    }
}