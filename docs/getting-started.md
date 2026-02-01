# Getting Started with Served.SDK

This guide will help you install and configure the Served.SDK for your .NET application.

---

## Prerequisites

- .NET 8.0 or later
- A Served/UnifiedHQ account
- An API key ([get one here](https://unifiedhq.ai/app/settings/api-keys))

---

## Installation

### Via .NET CLI

```bash
dotnet add package Served.SDK
```

### Via Package Manager Console

```powershell
Install-Package Served.SDK
```

### Via PackageReference

```xml
<PackageReference Include="Served.SDK" Version="2026.*" />
```

---

## Basic Setup

### 1. Create a Client

```csharp
using Served.SDK.Client;

var client = new ServedClientBuilder()
    .WithApiKey("your-api-key")
    .WithTenant("your-workspace-slug")
    .Build();
```

### 2. Make Your First Request

```csharp
// Get all projects
var projects = await client.ProjectManagement.Projects.GetAllAsync();

foreach (var project in projects)
{
    Console.WriteLine($"{project.Id}: {project.Name}");
}
```

---

## Configuration Options

### Builder Methods

| Method | Description |
|--------|-------------|
| `WithApiKey(string)` | Set API key for authentication |
| `WithTenant(string)` | Set workspace/tenant slug |
| `WithBaseUrl(string)` | Override base URL (default: apis.unifiedhq.ai) |
| `WithTimeout(TimeSpan)` | Set HTTP timeout |
| `WithRetry(int)` | Set retry count for failed requests |

### Example with All Options

```csharp
var client = new ServedClientBuilder()
    .WithApiKey(Environment.GetEnvironmentVariable("SERVED_API_KEY"))
    .WithTenant("my-company")
    .WithBaseUrl("https://apis.unifiedhq.ai")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRetry(3)
    .Build();
```

---

## ASP.NET Core Integration

### Register in DI Container

```csharp
// Program.cs
builder.Services.AddServedClient(options =>
{
    options.ApiKey = builder.Configuration["Served:ApiKey"];
    options.Tenant = builder.Configuration["Served:Tenant"];
});
```

### Configuration in appsettings.json

```json
{
  "Served": {
    "ApiKey": "your-api-key",
    "Tenant": "your-workspace"
  }
}
```

### Inject and Use

```csharp
public class ProjectService
{
    private readonly IServedClient _client;

    public ProjectService(IServedClient client)
    {
        _client = client;
    }

    public async Task<List<ProjectSummary>> GetActiveProjectsAsync()
    {
        return await _client.ProjectManagement.Projects.GetAllAsync(
            new ProjectQueryParams { Status = "active" }
        );
    }
}
```

---

## Error Handling

```csharp
try
{
    var project = await client.ProjectManagement.Projects.GetAsync(123);
}
catch (ServedApiException ex)
{
    Console.WriteLine($"API Error: {ex.StatusCode} - {ex.Message}");

    if (ex.StatusCode == 404)
    {
        Console.WriteLine("Project not found");
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
```

---

## Next Steps

- [Authentication Guide](./authentication.md) - Learn about authentication options
- [API Reference](./api-reference/) - Explore all available APIs
- [Examples](./examples/) - See common patterns and use cases

---

## Need Help?

- [Documentation](https://docs.served.dk)
- [Discord Community](https://discord.gg/unifiedhq)
- [GitHub Issues](https://github.com/UnifiedHQ/served-sdk/issues)
