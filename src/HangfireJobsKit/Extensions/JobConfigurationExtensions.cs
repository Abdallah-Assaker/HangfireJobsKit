namespace HangfireJobsKit.Extensions;

using System.Reflection;
using HangfireJobsKit.Abstractions;
using HangfireJobsKit.Configuration;

/// <summary>
/// Extension methods for job configuration
/// </summary>
public static class JobConfigurationExtensions
{
    /// <summary>
    /// Gets the configured queue for a job
    /// </summary>
    /// <typeparam name="TJob">Type of job</typeparam>
    /// <param name="job">The job instance</param>
    /// <returns>The configured queue name or "default" if not specified</returns>
    public static string GetJobConfigurationQueue<TJob>(this TJob job) 
        where TJob : IJob
    {
        var jobType = job.GetType();
        var jobConfig = jobType.GetCustomAttribute<JobConfigurationAttribute>();
        return jobConfig?.Queue ?? "default";
    }
    
    /// <summary>
    /// Gets the configured display name for a job
    /// </summary>
    /// <typeparam name="TJob">Type of job</typeparam>
    /// <param name="job">The job instance</param>
    /// <returns>The configured display name or the job type name if not specified</returns>
    public static string GetJobConfigurationDisplayName<TJob>(this TJob job) 
        where TJob : IJob
    {
        var jobType = job.GetType();
        var jobConfig = jobType.GetCustomAttribute<JobConfigurationAttribute>();
        return jobConfig?.DisplayName ?? jobType.Name;
    }
}