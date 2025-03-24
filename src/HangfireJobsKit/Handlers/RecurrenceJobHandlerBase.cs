using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Abstractions.Handlers;
using Microsoft.Extensions.Logging;

namespace HangfireJobsKit.Handlers;

/// <summary>
/// Base implementation for recurrence job handlers
/// </summary>
/// <typeparam name="TJob">Type of recurrence job this handler can process</typeparam>
public abstract class RecurrenceJobHandlerBase<TJob> : BackgroundJobHandlerBase<TJob>, IRecurrenceJobHandlerBase<TJob>
    where TJob : IRecurrenceJob;