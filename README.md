# Served.SDK

The official .NET SDK for the [Served](https://served.dk) and [UnifiedHQ](https://unifiedhq.ai) platforms.

[![NuGet](https://img.shields.io/nuget/v/Served.SDK.svg)](https://www.nuget.org/packages/Served.SDK)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License: NON-AI-MIT](https://img.shields.io/badge/License-NON--AI--MIT-red.svg)](./LICENSE)

---

## Installation

```bash
dotnet add package Served.SDK
```

Or via Package Manager:

```powershell
Install-Package Served.SDK
```

---

## Quick Start

```csharp
using Served.SDK.Client;

// Create client with API key
var client = new ServedClientBuilder()
    .WithApiKey("your-api-key")
    .WithTenant("your-workspace")
    .Build();

// List projects
var projects = await client.ProjectManagement.Projects.GetAllAsync();

// Create a task
var task = await client.ProjectManagement.Tasks.CreateAsync(new CreateTaskRequest
{
    ProjectId = 123,
    Name = "My Task",
    Description = "Task description"
});

// Log time
await client.Registration.TimeRegistrations.CreateAsync(new CreateTimeRegistrationRequest
{
    TaskId = task.Id,
    Hours = 2.5m,
    Date = DateTime.Today,
    Description = "Working on feature"
});
```

---

## Documentation

| Guide | Description |
|-------|-------------|
| [Getting Started](./docs/getting-started.md) | Installation and first steps |
| [Authentication](./docs/authentication.md) | API keys, tokens, and OAuth |
| [API Reference](./docs/api-reference/) | Complete API documentation |
| [Examples](./docs/examples/) | Common use cases and patterns |

---

## API Modules

The SDK is organized into logical modules:

| Module | Description | Access |
|--------|-------------|--------|
| **ProjectManagement** | Projects, tasks, sprints | `client.ProjectManagement` |
| **Registration** | Time tracking | `client.Registration` |
| **Companies** | Customers, contacts | `client.Companies` |
| **FinanceModule** | Invoices, billing | `client.FinanceModule` |
| **Identity** | Users, API keys, employees | `client.Identity` |
| **Calendar** | Agreements, appointments | `client.Calendar` |
| **DevOpsModule** | Git repos, PRs, pipelines | `client.DevOpsModule` |
| **SalesModule** | CRM, deals, pipelines | `client.SalesModule` |
| **BoardModule** | Kanban boards, sheets | `client.BoardModule` |
| **Reporting** | Dashboards, datasources | `client.Reporting` |

---

## Configuration

### Basic Configuration

```csharp
var client = new ServedClientBuilder()
    .WithApiKey("your-api-key")
    .WithTenant("your-workspace")
    .WithBaseUrl("https://apis.unifiedhq.ai")  // Optional
    .Build();
```

### ASP.NET Core Integration

```csharp
// In Program.cs or Startup.cs
services.AddServedClient(options =>
{
    options.ApiKey = Configuration["Served:ApiKey"];
    options.Tenant = Configuration["Served:Tenant"];
});

// Inject via DI
public class MyService
{
    private readonly IServedClient _client;

    public MyService(IServedClient client)
    {
        _client = client;
    }
}
```

---

## Environments

| Environment | Base URL |
|-------------|----------|
| **Production** | `https://apis.unifiedhq.ai` |
| **Local Dev** | `http://localhost:5010` |

---

## Get Your API Key

1. Log in to [unifiedhq.ai](https://unifiedhq.ai)
2. Go to **Settings** > **API Keys**
3. Create a new API key with required scopes
4. Copy and store securely

---

## Support

| Resource | Link |
|----------|------|
| **Documentation** | [docs.served.dk](https://docs.served.dk) |
| **API Reference** | [unifiedhq.ai/docs/api](https://unifiedhq.ai/docs/api) |
| **Issues** | [GitHub Issues](https://github.com/UnifiedHQ/served-sdk/issues) |
| **Discord** | [UnifiedHQ Community](https://discord.gg/unifiedhq) |

---

## License

This documentation is licensed under [NON-AI-MIT](./LICENSE).

The SDK package is available via [NuGet](https://www.nuget.org/packages/Served.SDK).

> **Notice:** This repository contains documentation only. The SDK source code is proprietary.
> AI training on this content is prohibited under the license terms.

---

Built with care by [UnifiedHQ](https://unifiedhq.ai)
