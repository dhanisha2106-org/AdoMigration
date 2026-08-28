# AdoToGitHub Wrapper

A comprehensive C# .NET wrapper for the GitHub `ado2gh` CLI tool with additional features for migrating Azure DevOps repositories and work items to GitHub.

## Features

✅ **Repository Migration**
- Lock, migrate, and disable Azure DevOps repositories
- Queue migrations and monitor status
- Download migration logs
- Batch migrations with parallel execution

✅ **Work Items Migration**
- Export work items from Azure Boards
- Create GitHub Issues with full context
- Preserve comments, attachments, and metadata
- Map work item types to GitHub labels

✅ **Validation & Preflight Checks**
- Verify repository existence
- Check permissions
- Validate repository size
- Pre-migration warnings

✅ **Enhanced Features**
- Async/await throughout
- Proper error handling
- Progress tracking
- Cancellation token support

## Prerequisites

1. Install GitHub CLI with ado2gh extension:
   ```bash
   gh extension install github/gh-ado2gh
   ```

2. Set environment variables:
   ```bash
   $env:ADO_PAT = "your-azure-devops-pat"
   $env:GH_PAT = "your-github-pat"
   ```

## Installation

```bash
dotnet add package AdoToGitHubWrapper
```

Or build from source:
```bash
cd AdoToGitHubWrapper
dotnet build
```

## Usage

### Basic Migration

```csharp
var wrapper = new AdoToGitHubWrapper(
    adoOrg: "dhanishasangeeth0060",
    githubOrg: "dhanisha-org",
    adoPat: Environment.GetEnvironmentVariable("ADO_PAT")!,
    githubPat: Environment.GetEnvironmentVariable("GH_PAT")!
);

// Migrate repository
var result = await wrapper.MigrateRepositoryAsync(
    teamProject: "AdoMigration",
    repo: "AdoMigration",
    targetRepo: "AdoMigration",
    visibility: "public"
);

if (result.Success)
{
    Console.WriteLine($"Migration ID: {result.MigrationId}");
}
```

### Validate Before Migration

```csharp
var validation = await wrapper.ValidateMigrationAsync(
    teamProject: "AdoMigration",
    repo: "AdoMigration",
    targetRepo: "AdoMigration"
);

if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
    {
        Console.WriteLine(error);
    }
}
```

### Monitor Migration Status

```csharp
var status = await wrapper.GetMigrationStatusAsync(migrationId);

while (status.State == "IN_PROGRESS" || status.State == "QUEUED")
{
    await Task.Delay(TimeSpan.FromSeconds(30));
    status = await wrapper.GetMigrationStatusAsync(migrationId);
    Console.WriteLine($"Status: {status.State}");
}
```

### Migrate Work Items

```csharp
var workItemResult = await wrapper.MigrateWorkItemsAsync(
    teamProject: "AdoMigration",
    githubRepo: "AdoMigration"
);

Console.WriteLine($"Migrated: {workItemResult.SuccessfullyMigrated}/{workItemResult.TotalItems}");
```

### Batch Migration

```csharp
var repositories = new List<(string, string, string, string)>
{
    ("Project1", "Repo1", "new-repo-1", "private"),
    ("Project2", "Repo2", "new-repo-2", "public")
};

var results = await wrapper.MigrateMultipleRepositoriesAsync(
    repositories,
    maxParallel: 3
);
```

## Integration with UI

### Blazor Server Example

```csharp
@page "/migrate"
@inject AdoToGitHubWrapper Wrapper

<h3>Repository Migration</h3>

<EditForm Model="@model" OnValidSubmit="@MigrateAsync">
    <InputText @bind-Value="model.TeamProject" placeholder="Team Project" />
    <InputText @bind-Value="model.Repo" placeholder="Repository" />
    <InputText @bind-Value="model.TargetRepo" placeholder="Target Repo" />
    <button type="submit">Migrate</button>
</EditForm>

@if (isLoading)
{
    <p>Migration Status: @status</p>
}

@code {
    private MigrationModel model = new();
    private bool isLoading = false;
    private string status = "";

    private async Task MigrateAsync()
    {
        isLoading = true;
        var result = await Wrapper.MigrateRepositoryAsync(
            model.TeamProject,
            model.Repo,
            model.TargetRepo,
            "private"
        );

        if (result.Success)
        {
            await MonitorMigration(result.MigrationId!);
        }
        
        isLoading = false;
    }

    private async Task MonitorMigration(string migrationId)
    {
        var migrationStatus = await Wrapper.GetMigrationStatusAsync(migrationId);
        
        while (migrationStatus.State is "QUEUED" or "IN_PROGRESS")
        {
            status = migrationStatus.State;
            StateHasChanged();
            await Task.Delay(5000);
            migrationStatus = await Wrapper.GetMigrationStatusAsync(migrationId);
        }
        
        status = migrationStatus.State;
    }
}
```

### ASP.NET Core Web API Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly AdoToGitHubWrapper _wrapper;

    public MigrationController(AdoToGitHubWrapper wrapper)
    {
        _wrapper = wrapper;
    }

    [HttpPost("migrate")]
    public async Task<ActionResult<MigrationResult>> Migrate(
        [FromBody] MigrationRequest request)
    {
        var result = await _wrapper.MigrateRepositoryAsync(
            request.TeamProject,
            request.Repo,
            request.TargetRepo,
            request.Visibility
        );

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("status/{migrationId}")]
    public async Task<ActionResult<MigrationStatus>> GetStatus(string migrationId)
    {
        var status = await _wrapper.GetMigrationStatusAsync(migrationId);
        return Ok(status);
    }
}
```

## API Reference

See the [API Documentation](docs/API.md) for complete reference.

## License

MIT
