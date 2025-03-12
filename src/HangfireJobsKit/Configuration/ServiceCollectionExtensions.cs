using Hangfire;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Filters.Core;
using HangfireJobsKit.Filters.DefaultFilters;
using HangfireJobsKit.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HangfireJobsKit.Configuration;

/// <summary>
/// Extension methods for registering HangfireJobsKit services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds HangfireJobsKit core services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHangfireJobsKit(this IServiceCollection services)
    {
        // Register core services
        services.AddSingleton<JobActivator, ContextAwareJobActivator>();
        services.AddSingleton<JobConfigurationFilter>();
        services.AddSingleton<HangfireJobFilter>();
        
        // Register job managers
        services.AddScoped<IJobManager, HangfireJobManager>();
        services.AddScoped<IRecurrenceJobManager, HangfireRecurrenceJobManager>();
        services.AddScoped<IDelayedJobManager, HangfireDelayedJobManager>();
        
        // Register default filters
        services.AddScoped<IHangfireJobFilter, ErrorLoggingFilter>();
        
        return services;
    }

    /// <summary>
    /// Configures Hangfire with recommended settings for HangfireJobsKit
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="connectionString">The SQL Server connection string for Hangfire storage</param>
    /// <param name="configure">Optional configuration action for Hangfire</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHangfireWithJobsKit(
        this IServiceCollection services,
        string connectionString,
        Action<IGlobalConfiguration>? configure = null)
    {
        // Add HangfireJobsKit services
        services.AddHangfireJobsKit();
        
        // Configure Hangfire with recommended settings
        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSerializerSettings(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                })
                .UseSqlServerStorage(connectionString)
                .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })
                .UseColouredConsoleLogProvider();
                
            // Allow additional configuration if needed
            configure?.Invoke(config);
        });
        
        return services;
    }

    /// <summary>
    /// Configures the Hangfire app with HangfireJobsKit filters
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseHangfireJobsKit(this IApplicationBuilder app)
    {
        // Register global filters for HangfireJobsKit
        var jobConfigFilter = app.ApplicationServices.GetRequiredService<JobConfigurationFilter>();
        var hangfireJobFilter = app.ApplicationServices.GetRequiredService<HangfireJobFilter>();
        
        GlobalJobFilters.Filters.Add(jobConfigFilter, order: -10); // Make sure it runs first
        GlobalJobFilters.Filters.Add(hangfireJobFilter, order: 0);  // Then the main filter
        
        return app;
    }
    
    /// <summary>
    /// Registers a custom filter for HangfireJobsKit
    /// </summary>
    /// <typeparam name="TFilter">The filter type to register</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHangfireJobsKitFilter<TFilter>(this IServiceCollection services)
        where TFilter : class, IHangfireJobFilter
    {
        services.AddScoped<IHangfireJobFilter, TFilter>();
        return services;
    }
    
    /// <summary>
    /// Adds a Hangfire server with default configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="name">The server name</param>
    /// <param name="queues">The queues to process</param>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHangfireJobsKitServer(
        this IServiceCollection services,
        string name,
        string[] queues,
        Action<BackgroundJobServerOptions>? configure = null)
    {
        services.AddHangfireServer(options =>
        {
            options.ServerName = name;
            options.Queues = queues;
            
            // Apply additional configuration if provided
            configure?.Invoke(options);
        });
        
        return services;
    }
}