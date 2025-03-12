namespace HangfireJobsKit.Filters.Core;

using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Job activator that leverages the DI container scope created by the filters
/// </summary>
public class ContextAwareJobActivator : JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new instance of ContextAwareJobActivator
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public ContextAwareJobActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Begins a scope for job activation
    /// </summary>
    /// <param name="context">The perform context</param>
    /// <returns>A job activator scope</returns>
    public override JobActivatorScope BeginScope(PerformContext context)
    {
        if (context.Items.TryGetValue("HangfireScope", out var scopeObj) &&
            scopeObj is IServiceScope existingScope)
        {
            // Existing scope from filter - don't dispose
            return new ExistingDependencyScope(existingScope, shouldDispose: false);
        }
        // New fallback scope - mark for disposal
        var newScope = _serviceProvider.CreateScope();
        return new ExistingDependencyScope(newScope, shouldDispose: true);
    }

    private class ExistingDependencyScope : JobActivatorScope
    {
        private readonly IServiceScope _scope;
        private readonly bool _shouldDispose;

        public ExistingDependencyScope(IServiceScope scope, bool shouldDispose)
        {
            _scope = scope;
            _shouldDispose = shouldDispose;
        }

        public override object Resolve(Type type)
        {
            return _scope.ServiceProvider.GetRequiredService(type);
        }

        public override void DisposeScope()
        {
            if (_shouldDispose)
            {
                _scope.Dispose();
            }
        }
    }
}

