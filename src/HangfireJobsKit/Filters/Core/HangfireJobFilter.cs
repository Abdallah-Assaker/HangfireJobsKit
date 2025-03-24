using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using HangfireJobsKit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireJobsKit.Filters.Core;

/// <summary>
/// Main filter that orchestrates the execution of all custom filters
/// </summary>
internal class HangfireJobFilter : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
{
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Creates a new instance of HangfireJobFilter
    /// </summary>
    /// <param name="scopeFactory">The service scope factory</param>
    public HangfireJobFilter(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Called when a background job is creating
    /// </summary>
    public void OnCreating(CreatingContext context)
    {
    }

    /// <summary>
    /// Called when a background job has been created
    /// </summary>
    public void OnCreated(CreatedContext context)
    {
    }

    /// <summary>
    /// Called before the execution of a background job
    /// </summary>
    public void OnPerforming(PerformingContext context)
    {
        var scope = _scopeFactory.CreateScope();
        context.Items["HangfireScope"] = scope;

        var ascendingSortedHangfireJobFilters = scope
            .ServiceProvider
            .GetServices<IHangfireJobFilter>()?
            .OrderBy(x => x.ExecutionOrder) ?? Enumerable.Empty<IHangfireJobFilter>();

        // Initialize context for all filters
        foreach (var hangfireJobFilter in ascendingSortedHangfireJobFilters)
        {
            hangfireJobFilter.OnPerforming(context);
        }
    }

    /// <summary>
    /// Called after the execution of a background job
    /// </summary>
    public void OnPerformed(PerformedContext context)
    {
        if (context.Items["HangfireScope"] is not IServiceScope scope) return;
        
        var descendingSortedHangfireJobFilters = scope
            .ServiceProvider
            .GetServices<IHangfireJobFilter>()?
            .OrderByDescending(x => x.ExecutionOrder) ?? Enumerable.Empty<IHangfireJobFilter>();

        // Execute all filters in reverse order
        foreach (var hangfireJobFilter in descendingSortedHangfireJobFilters)
        {
            hangfireJobFilter.OnPerformed(context);
        }

        scope.Dispose();
    }
    
    /// <summary>
    /// Called during state election for a job
    /// </summary>
    public void OnStateElection(ElectStateContext context) { }

    /// <summary>
    /// Called after a state was successfully applied
    /// </summary>
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }

    /// <summary>
    /// Called when a state application failed
    /// </summary>
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
}