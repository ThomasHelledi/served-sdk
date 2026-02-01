# API Reference

Complete reference for all Served.SDK APIs.

---

## Module Overview

| Module | Description | Documentation |
|--------|-------------|---------------|
| **ProjectManagement** | Projects, tasks, sprints | [Projects](./projects.md), [Tasks](./tasks.md) |
| **Registration** | Time tracking | [Time Registration](./time-registration.md) |
| **Companies** | Customers, contacts | [Customers](./customers.md) |
| **FinanceModule** | Invoices, billing | [Finance](./finance.md) |
| **Identity** | Users, API keys, employees | [Identity](./identity.md) |
| **Calendar** | Agreements, appointments | [Calendar](./calendar.md) |
| **DevOpsModule** | Git repos, PRs, pipelines | [DevOps](./devops.md) |
| **SalesModule** | CRM, deals, pipelines | [Sales](./sales.md) |
| **BoardModule** | Kanban boards, sheets | [Boards](./boards.md) |
| **Reporting** | Dashboards, datasources | [Reporting](./reporting.md) |

---

## Quick Access Patterns

### Module-Based (Recommended)

```csharp
// Projects
var projects = await client.ProjectManagement.Projects.GetAllAsync();

// Tasks
var tasks = await client.ProjectManagement.Tasks.GetByProjectAsync(123);

// Time
var time = await client.Registration.TimeRegistrations.GetAllAsync();

// Customers
var customers = await client.Companies.Customers.GetAllAsync();

// Finance
var invoices = await client.FinanceModule.Invoices.GetAllAsync();

// DevOps
var repos = await client.DevOpsModule.Repositories.GetAllAsync();
```

### Legacy Access (Backwards Compatible)

```csharp
// Still supported but module-based is preferred
var projects = await client.Projects.GetAllAsync();
var tasks = await client.Tasks.GetAllAsync();
var time = await client.TimeRegistrations.GetAllAsync();
```

---

## Common Operations

### CRUD Pattern

All resource clients follow a consistent CRUD pattern:

```csharp
// Create
var item = await client.Module.Resource.CreateAsync(request);

// Read
var item = await client.Module.Resource.GetAsync(id);
var items = await client.Module.Resource.GetAllAsync(query);

// Update
var updated = await client.Module.Resource.UpdateAsync(id, request);

// Delete
await client.Module.Resource.DeleteAsync(id);
```

### Bulk Operations

Most resources support bulk operations:

```csharp
// Bulk create
var result = await client.Module.Resource.CreateBulkAsync(bulkRequest);

// Bulk update
var result = await client.Module.Resource.UpdateBulkAsync(bulkRequest);

// Bulk delete
var result = await client.Module.Resource.DeleteBulkAsync(bulkRequest);
```

### Query Operations

Efficient querying patterns:

```csharp
// Get only IDs
var ids = await client.Module.Resource.GetKeysAsync(query);

// Get multiple by IDs
var items = await client.Module.Resource.GetRangeAsync(ids);

// Search
var results = await client.Module.Resource.SearchAsync("term", take: 20);
```

---

## Error Handling

All API methods can throw:

| Exception | Cause |
|-----------|-------|
| `ServedApiException` | API returned error response |
| `HttpRequestException` | Network or connectivity issue |
| `JsonException` | Response parsing failed |

```csharp
try
{
    var project = await client.ProjectManagement.Projects.GetAsync(123);
}
catch (ServedApiException ex) when (ex.StatusCode == 404)
{
    // Handle not found
}
catch (ServedApiException ex) when (ex.StatusCode == 403)
{
    // Handle forbidden (scope/permission issue)
}
```

---

## Rate Limiting

The API enforces rate limits. The SDK handles this automatically with configurable retry:

```csharp
var client = new ServedClientBuilder()
    .WithApiKey("key")
    .WithRetry(3)  // Retry up to 3 times with exponential backoff
    .Build();
```

---

## Pagination

For large datasets, use pagination:

```csharp
var allProjects = new List<ProjectSummary>();
var skip = 0;
const int take = 100;

while (true)
{
    var batch = await client.ProjectManagement.Projects.GetAllAsync(
        new ProjectQueryParams { Skip = skip, Take = take }
    );

    if (batch.Count == 0) break;

    allProjects.AddRange(batch);
    skip += take;
}
```
