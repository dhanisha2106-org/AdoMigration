# PWC.Ado2GitHubTool - Simplified Architecture

PWC.Ado2GitHubTool is a **backend wrapper for the GitHub CLI ado2gh tool**, following Clean Architecture with a simplified 2-project structure.

## Architecture Overview

The solution follows a **layered architecture** with clear separation of concerns:

```
PWC.Ado2GitHubTool.Core (Domain Layer - Pure interfaces and models)
PWC.Ado2GitHubTool (Implementation - CLI execution and business logic)
```

## Project Structure

### PWC.Ado2GitHubTool.Core

**Interfaces & Models** - Defines contracts and domain models.

**Interfaces:**
- `IAdo2GhCliExecutor` - Abstract CLI execution (Process.Start)
- `IAdo2GhRepositoryMigrator` - Orchestrates repository migration

**Models:**
- `Ado2GhMigrationOptions` - Configuration for migration
- `MigrationResult` - Result of migration operation
- `ValidationResult` - Validation result

### PWC.Ado2GitHubTool

**CLI Execution & Business Logic** - Combines infrastructure and service layers.

**Cli:**
- `Ado2GhCliExecutor` - Executes `gh ado2gh migrate-repo` command
  - Thread-safe with SemaphoreSlim
  - Real-time output streaming
  - Cancellation support
  - CLI availability validation

**Migrators:**
- `Ado2GhRepositoryMigrator` - Implements `IAdo2GhRepositoryMigrator`
  - Single and batch repository migration
  - Concurrency control (max 3 parallel)
  - Progress callbacks
  - Pre-migration validation

**DependencyInjection:**
- Master DI configuration combining all registrations

## Benefits of Simplified Architecture

1. **Testability** - Can mock `IAdo2GhCliExecutor` for unit tests
2. **Maintainability** - Clear separation without over-engineering
3. **Simplicity** - Fewer projects to manage for a CLI wrapper
4. **Consistency** - Core remains pure (no framework dependencies)
5. **Integration** - Clean DI registration in UI

## Usage

### 1. Register in Dependency Injection

```csharp
// In Program.cs or Startup.cs
builder.Services.AddAdo2GitHubTool();
```

This automatically registers:
- `IAdo2GhCliExecutor` → `Ado2GhCliExecutor` (Singleton)
- `IAdo2GhRepositoryMigrator` → `Ado2GhRepositoryMigrator` (Scoped)

### 2. Inject and Use

```csharp
public class MyService
{
    private readonly IAdo2GhRepositoryMigrator _migrator;

    public MyService(IAdo2GhRepositoryMigrator migrator)
    {
        _migrator = migrator;
    }

    public async Task MigrateAsync()
    {
        var options = new Ado2GhMigrationOptions
        {
            AdoOrg = "myorg",
            AdoTeamProject = "myproject",
            AdoRepo = "myrepo",
            GithubOrg = "mygithub",
            AdoPat = "***",
            GithubPat = "***",
            Wait = true
        };

        var result = await _migrator.MigrateRepositoryAsync(
            options,
            progress => Console.WriteLine(progress));

        if (result.Success)
        {
            Console.WriteLine($"Migrated {result.RepositoryName} successfully!");
        }
    }
}
```

### 3. Batch Migration

```csharp
var repos = new[] { "repo1", "repo2", "repo3" };
var results = await _migrator.MigrateMultipleRepositoriesAsync(
    repos,
    baseOptions,
    maxConcurrency: 3,
    progress => _logger.LogInformation(progress));

var successful = results.Count(r => r.Success);
Console.WriteLine($"Migrated {successful}/{repos.Length} repositories");
```

## Prerequisites

- **GitHub CLI (`gh`)** - Install from https://cli.github.com/
- **ado2gh extension** - Install with: `gh extension install github/gh-ado2gh`
- **Azure DevOps PAT** - With Code (Read) permissions
- **GitHub PAT** - With repo and admin:org permissions

## Migration Flow

1. **Validation** - Check CLI availability, validate options
2. **CLI Execution** - Execute `gh ado2gh migrate-repo`
3. **Progress Streaming** - Real-time output via callbacks
4. **Result Collection** - Return MigrationResult with status

## Dependency Injection Pattern

### Infrastructure Layer

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddAdo2GitHubToolInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IAdo2GhCliExecutor, Ado2GhCliExecutor>();
        return services;
    }
}
```

### Services Layer

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddAdo2GitHubToolServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAdo2GhRepositoryMigrator, Ado2GhRepositoryMigrator>();
        return services;
    }
}
```

### Composition Root

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddAdo2GitHubTool(
        this IServiceCollection services)
    {
        services.AddAdo2GitHubToolInfrastructure();
        services.AddAdo2GitHubToolServices();
        return services;
    }
}
```

## Related Projects

- **PwC.ADO2GitHub.Core** - Canonical models and interfaces
- **PwC.ADO2GitHub.Services** - API-based migrators (direct HTTP)
- **PwC.ADO2GitHub.UI** - Blazor UI with toggle for CLI vs. API

### UI Integration

When the UI toggle "Use ado2gh CLI" is **ON**, PWC.Ado2GitHubTool is used.  
When **OFF**, PwC.ADO2GitHub.Services is used for direct API migration.

## Comparison: Before vs. After Refactoring

### Before (Monolithic)

```
PWC.Ado2GitHubTool/
├── Ado2GitHubTool.cs          (400 lines - everything)
├── Models/
│   ├── WorkItem.cs
│   └── MigrationResult.cs
└── Services/
    ├── AzureDevOpsService.cs
    └── GitHubService.cs
```

**Issues:**
- ❌ No interfaces → tightly coupled
- ❌ No dependency injection
- ❌ Difficult to test
- ❌ Mixed concerns

### After (Clean Architecture)

```
PWC.Ado2GitHubTool.Core/           [Interfaces & Models]
├── Interfaces/
│   ├── IAdo2GhRepositoryMigrator.cs
│   └── IAdo2GhCliExecutor.cs
└── Models/
    ├── Ado2GhMigrationOptions.cs
    ├── MigrationResult.cs
    └── ValidationResult.cs

PWC.Ado2GitHubTool.Infrastructure/ [External I/O]
├── Cli/
│   └── Ado2GhCliExecutor.cs
└── DependencyInjection.cs

PWC.Ado2GitHubTool.Services/       [Business Logic]
├── Migrators/
│   └── Ado2GhRepositoryMigrator.cs
└── DependencyInjection.cs

PWC.Ado2GitHubTool/                 [Composition Root]
└── DependencyInjection.cs
```

**Benefits:**
- ✅ Interface-driven → loosely coupled
- ✅ Dependency injection → testable
- ✅ Layered architecture → maintainable
- ✅ Clear separation of concerns
