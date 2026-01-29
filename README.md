# Served.SDK

[![NuGet](https://img.shields.io/nuget/v/Served.SDK.svg)](https://www.nuget.org/packages/Served.SDK)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Served.SDK.svg)](https://www.nuget.org/packages/Served.SDK)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

The official .NET SDK for the **Served API** by [UnifiedHQ](https://unifiedhq.ai). This library provides a strongly-typed, modern fluent client for interacting with the Served platform, covering Project Management, Task Tracking, Time Registration, Finance, DevOps, and more.

## Links

| Resource | URL |
|----------|-----|
| **UnifiedHQ Platform** | [unifiedhq.ai](https://unifiedhq.ai) |
| **Forge DevOps** | [forge.unifiedhq.ai](https://forge.unifiedhq.ai) |
| **API Documentation** | [unifiedhq.ai/docs/api](https://unifiedhq.ai/docs/api) |
| **MCP Server** | [github.com/ThomasHelledi/served-mcp](https://github.com/ThomasHelledi/served-mcp) |
| **NuGet Package** | [nuget.org/packages/Served.SDK](https://www.nuget.org/packages/Served.SDK) |

## Features

- **Module-Based API**: Organized access through domain modules (e.g., `client.ProjectManagement.Projects`).
- **Generic & Abstract**: Built on reusable base classes for consistent CRUD operations.
- **Strongly Typed**: Full type safety for all Served entities (Projects, Tasks, Invoices, etc.).
- **Modern Async**: Built from the ground up with `async/await`.
- **Bulk Operations**: Support for batch create, update, and delete operations.
- **Error Handling**: Custom `ServedApiException` provides detailed error context.
- **Backwards Compatible**: Legacy client access patterns still supported.
- **Tracing & Observability**: Built-in OpenTelemetry support with Forge platform integration.
- **Fork & Extend**: Open source - fork and run your own pipelines.

## Installation

```bash
dotnet add package Served.SDK
```

## Quick Start

```csharp
using Served.SDK.Client;

// Initialize with UnifiedHQ infrastructure (default)
var client = new ServedClientBuilder()
    .WithToken("YOUR_API_TOKEN")
    .WithTenant("YOUR_TENANT_SLUG")
    .WithTracing(options => options.EnableForge = true)  // Enable Forge analytics
    .Build();

// Get projects
var projects = await client.ProjectManagement.Projects.GetAllAsync();

// Create a task
var task = await client.ProjectManagement.Tasks.CreateAsync(new CreateTaskRequest
{
    Name = "My Task",
    ProjectId = projects.First().Id
});

// Log time
await client.Registration.TimeRegistrations.CreateAsync(new CreateTimeRegistrationRequest
{
    TaskId = task.Id,
    Minutes = 60,
    Description = "Working on task"
});
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `SERVED_API_URL` | API base URL | `https://apis.unifiedhq.ai` |
| `SERVED_TRACING_ENABLED` | Enable/disable tracing | `false` |
| `SERVED_SERVICE_NAME` | Service identifier | `served-sdk-client` |
| `FORGE_API_KEY` | Forge platform API key | - |
| `FORGE_ENDPOINT` | Forge telemetry endpoint | `https://apis.unifiedhq.ai/v1/telemetry` |

### Initialization Options

```csharp
// Simple initialization (uses UnifiedHQ infrastructure)
var client = new ServedClient(
    baseUrl: ServedClient.DefaultApiUrl,  // https://apis.unifiedhq.ai
    token: "YOUR_API_TOKEN",
    tenant: "YOUR_TENANT_SLUG"
);

// Builder pattern with full control
var client = new ServedClientBuilder()
    .WithBaseUrl("https://apis.unifiedhq.ai")
    .WithToken(token)
    .WithTenant("my-workspace")
    .WithTracing(options =>
    {
        options.ServiceName = "my-application";
        options.EnableForge = true;
        options.SamplingRate = 0.1;
    })
    .Build();

// Dependency injection for ASP.NET Core
services.AddHttpClient<IServedClient, ServedClient>(client =>
{
    client.BaseAddress = new Uri("https://apis.unifiedhq.ai");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TOKEN");
});
```

## API Modules

| Module | Resources | Description |
|--------|-----------|-------------|
| `ProjectManagement` | Projects, Tasks | Project and task management |
| `DevOpsModule` | Repositories, PullRequests, Pipelines | DevOps integration |
| `FinanceModule` | Invoices | Invoice management |
| `SalesModule` | Pipelines, Deals | Sales pipeline and deals |
| `Registration` | TimeRegistrations | Time tracking |
| `Companies` | Customers | Customer management |
| `Identity` | Employees, ApiKeys | User and API key management |
| `Calendar` | Agreements | Appointments and agreements |
| `Board` | Boards, Sheets | Kanban boards |
| `Reporting` | Dashboards, Datasources | Reports and dashboards |
| `Tenant` | Tenants, Workspaces | Multi-tenant management |

## Tracing & Analytics

The SDK includes built-in tracing with OpenTelemetry support and [Forge](https://forge.unifiedhq.ai) platform integration.

### Enable Tracing

```csharp
var client = new ServedClient("https://apis.unifiedhq.ai", token)
    .EnableTracing();

// Or with builder for full control
var client = new ServedClientBuilder()
    .WithToken(token)
    .WithTracing(options =>
    {
        options.ServiceName = "my-application";
        options.EnableForge = true;
        options.SamplingRate = 0.1;
    })
    .Build();
```

### View Analytics

Once tracing is enabled, view your analytics at:
- **Forge Dashboard**: [forge.unifiedhq.ai/analytics](https://forge.unifiedhq.ai/analytics)
- **Your Workspace**: [unifiedhq.ai/app/analytics](https://unifiedhq.ai/app/analytics)

### Custom Events

```csharp
client.Tracer?.RecordEvent(new TelemetryEvent
{
    Type = TelemetryEventType.Custom,
    Name = "invoice.generated",
    Attributes = new Dictionary<string, object>
    {
        ["invoice.id"] = invoiceId,
        ["invoice.amount"] = amount
    }
});
```

## Fork & Extend

This SDK is open source. Fork it to:

1. **Run Your Own Pipelines**: Fork the repo and run CI/CD in your environment
2. **Extend Functionality**: Add custom modules for your use case
3. **Contribute Back**: Submit PRs to improve the SDK

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/served-sdk.git

# Build locally
dotnet build

# Run tests
dotnet test

# Create your own NuGet package
dotnet pack -c Release
```

## Error Handling

```csharp
try
{
    await client.ProjectManagement.Projects.GetAsync(99999);
}
catch (ServedApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("Project not found.");
}
catch (ServedApiException ex)
{
    Console.WriteLine($"API Error: {ex.StatusCode} - {ex.Content}");
}
```

## Support

- **Documentation**: [unifiedhq.ai/docs](https://unifiedhq.ai/docs)
- **Issues**: [GitHub Issues](https://github.com/ThomasHelledi/served-sdk/issues)
- **Discord**: [UnifiedHQ Community](https://discord.gg/unifiedhq)

## License

MIT License - see [LICENSE](LICENSE) for details.

---

Built with love by [UnifiedHQ](https://unifiedhq.ai)
