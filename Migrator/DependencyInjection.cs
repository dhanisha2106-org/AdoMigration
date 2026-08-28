using Microsoft.Extensions.DependencyInjection;
using PWC.Ado2GitHubTool.Cli;
using PWC.Ado2GitHubTool.Core.Interfaces;
using PWC.Ado2GitHubTool.Migrators;

namespace PWC.Ado2GitHubTool;

/// <summary>
/// Dependency injection configuration for PWC.Ado2GitHubTool.
/// Registers CLI executor and repository migrator.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add all PWC.Ado2GitHubTool components to the DI container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddAdo2GitHubTool(this IServiceCollection services)
    {
        // Register CLI executor as singleton (thread-safe with internal locking)
        services.AddSingleton<IAdo2GhCliExecutor, Ado2GhCliExecutor>();

        // Register migrator as scoped (per request)
        services.AddScoped<IAdo2GhRepositoryMigrator, Ado2GhRepositoryMigrator>();

        return services;
    }
}
