using Hangfire.Server;
using HangfireJobsKit.Models;

namespace HangfireJobsKit.Extensions;

/// <summary>
/// Extension methods for Hangfire context
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Gets a job parameter of the specified type from the performing context
    /// </summary>
    /// <typeparam name="T">Type of parameter to get</typeparam>
    /// <param name="context">The performing context</param>
    /// <returns>The parameter instance or null if not found</returns>
    public static T? GetAJobParameter<T>(this PerformingContext context) where T : class
    {
        return context.BackgroundJob.Job.Args
            .FirstOrDefault(arg => arg is T) as T;
    }
    
    /// <summary>
    /// Gets a job parameter of the specified type from the performed context
    /// </summary>
    /// <typeparam name="T">Type of parameter to get</typeparam>
    /// <param name="context">The performed context</param>
    /// <returns>The parameter instance or null if not found</returns>
    public static T? GetAJobParameter<T>(this PerformedContext context) where T : class
    {
        return context.BackgroundJob.Job.Args
            .FirstOrDefault(arg => arg is T) as T;
    }
    
    /// <summary>
    /// Gets the job context from the performing context
    /// </summary>
    /// <param name="context">The performing context</param>
    /// <returns>The job context or null if not found</returns>
    public static JobContext? GetJobContext(this PerformingContext context)
    {
        return GetAJobParameter<JobContext>(context);
    }
    
    /// <summary>
    /// Gets the job context from the performed context
    /// </summary>
    /// <param name="context">The performed context</param>
    /// <returns>The job context or null if not found</returns>
    public static JobContext? GetJobContext(this PerformedContext context)
    {
        return GetAJobParameter<JobContext>(context);
    }
}