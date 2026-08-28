# GraphQL Branch Information Console Application

A C# console application that queries a GraphQL API (GitHub) to retrieve branch information.

## Features

- Queries GitHub's GraphQL API
- Retrieves branch details including:
  - Branch name
  - Latest commit hash
  - Commit message
  - Commit date and time
  - Author information

## Prerequisites

- .NET 10.0 SDK or later
- GitHub Personal Access Token

## Setup

### 1. Get a GitHub Personal Access Token

1. Go to [GitHub Settings > Personal Access Tokens](https://github.com/settings/tokens)
2. Click "Generate new token" → "Generate new token (classic)"
3. Give it a descriptive name (e.g., "GraphQL Branch Info App")
4. Select the following scope:
   - `repo` (Full control of private repositories)
5. Click "Generate token"
6. **Copy the token immediately** (you won't see it again!)

### 2. Configure the Application

Edit the `appsettings.json` file:

```json
{
  "GraphQL": {
    "Endpoint": "https://api.github.com/graphql",
    "Token": "YOUR_GITHUB_TOKEN_HERE",
    "RepositoryOwner": "owner",
    "RepositoryName": "repo",
    "BranchName": "main"
  }
}
```

Replace:
- `YOUR_GITHUB_TOKEN_HERE` with your GitHub token
- `owner` with the repository owner (user or organization)
- `repo` with the repository name
- `main` with the branch you want to query

**Example:**
```json
{
  "GraphQL": {
    "Endpoint": "https://api.github.com/graphql",
    "Token": "ghp_abc123xyz789...",
    "RepositoryOwner": "microsoft",
    "RepositoryName": "vscode",
    "BranchName": "main"
  }
}
```

## Running the Application

```bash
dotnet run
```

## Example Output

```
=== GraphQL Branch Information Tool ===

Repository: microsoft/vscode
Branch: main

Fetching branch information...

Branch Details:
  Branch Name: main
  Latest Commit: a1b2c3d4e5f6...
  Message: Fix typo in documentation
  Committed: 2026-03-12T10:30:00Z
  Author: John Doe
  Email: john.doe@example.com
```

## Project Structure

- `Program.cs` - Main application logic
- `BranchInfo.cs` - Data models for GraphQL response
- `appsettings.json` - Configuration file
- `GraphQLBranchInfo.csproj` - Project file with dependencies

## Dependencies

- **GraphQL.Client** (6.1.0) - GraphQL client library
- **GraphQL.Client.Serializer.SystemTextJson** (6.1.0) - JSON serializer
- **Microsoft.Extensions.Configuration** (10.0.4) - Configuration management
- **Microsoft.Extensions.Configuration.Json** (10.0.4) - JSON configuration provider

## Customizing for Other GraphQL APIs

This application is configured for GitHub's GraphQL API, but you can adapt it for other GraphQL APIs:

1. Update the `Endpoint` in `appsettings.json`
2. Modify the authentication header in `Program.cs` (if different from Bearer token)
3. Update the GraphQL query to match your API's schema
4. Adjust the response models in `BranchInfo.cs`

## Security Notes

- **Never commit `appsettings.json` with real tokens to version control**
- Consider using environment variables or Azure Key Vault for production
- Add `appsettings.json` to `.gitignore`

## Troubleshooting

### "Please update the GitHub token"
- Ensure you've replaced `YOUR_GITHUB_TOKEN_HERE` with a valid token

### "GraphQL Errors: Could not resolve to a Repository"
- Check that the repository owner and name are correct
- Verify your token has access to the repository

### "Branch not found"
- Ensure the branch name is correct (case-sensitive)
- The application automatically adds `refs/heads/` prefix

## License

This is a sample application for educational purposes.
