# Projects API

Manage projects in your workspace.

Access via: `client.ProjectManagement.Projects`

---

## Methods

### GetAllAsync

Get all projects with optional filtering.

```csharp
var projects = await client.ProjectManagement.Projects.GetAllAsync();

// With filtering
var activeProjects = await client.ProjectManagement.Projects.GetAllAsync(
    new ProjectQueryParams
    {
        Status = "active",
        CustomerId = 123,
        Take = 50
    }
);
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `Status` | string | Filter by status (active, completed, archived) |
| `CustomerId` | int? | Filter by customer |
| `CategoryId` | int? | Filter by category |
| `Take` | int | Max results (default: 100) |
| `Skip` | int | Offset for pagination |

**Returns:** `List<ProjectSummary>`

---

### GetAsync

Get a single project by ID.

```csharp
var project = await client.ProjectManagement.Projects.GetAsync(123);

Console.WriteLine($"Project: {project.Name}");
Console.WriteLine($"Status: {project.Status}");
Console.WriteLine($"Budget: {project.BudgetHours}h");
```

**Returns:** `ProjectDetail`

---

### CreateAsync

Create a new project.

```csharp
var project = await client.ProjectManagement.Projects.CreateAsync(
    new CreateProjectRequest
    {
        Name = "Website Redesign",
        Description = "Complete website overhaul",
        CustomerId = 456,
        BudgetHours = 100,
        StartDate = DateTime.Today,
        DueDate = DateTime.Today.AddMonths(3)
    }
);

Console.WriteLine($"Created project #{project.Id}");
```

**Request Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `Name` | string | Yes | Project name |
| `Description` | string | No | Project description |
| `CustomerId` | int | No | Associated customer |
| `CategoryId` | int | No | Project category |
| `BudgetHours` | decimal | No | Hour budget |
| `StartDate` | DateTime | No | Start date |
| `DueDate` | DateTime | No | Due date |

---

### UpdateAsync

Update an existing project.

```csharp
var updated = await client.ProjectManagement.Projects.UpdateAsync(
    123,
    new UpdateProjectRequest
    {
        Name = "Website Redesign v2",
        Status = "active",
        BudgetHours = 150
    }
);
```

---

### DeleteAsync

Delete a project.

```csharp
await client.ProjectManagement.Projects.DeleteAsync(123);
```

> **Warning:** This permanently deletes the project and all associated tasks.

---

## Bulk Operations

### CreateBulkAsync

Create multiple projects at once.

```csharp
var result = await client.ProjectManagement.Projects.CreateBulkAsync(
    new BulkCreateProjectsRequest
    {
        Projects = new List<CreateProjectRequest>
        {
            new() { Name = "Project A", CustomerId = 1 },
            new() { Name = "Project B", CustomerId = 1 },
            new() { Name = "Project C", CustomerId = 2 }
        }
    }
);

Console.WriteLine($"Created: {result.Succeeded.Count}");
Console.WriteLine($"Failed: {result.Failed.Count}");
```

### UpdateBulkAsync

Update multiple projects.

```csharp
var result = await client.ProjectManagement.Projects.UpdateBulkAsync(
    new BulkUpdateProjectsRequest
    {
        Updates = new List<ProjectUpdate>
        {
            new() { Id = 1, Status = "active" },
            new() { Id = 2, Status = "active" },
            new() { Id = 3, Status = "completed" }
        }
    }
);
```

---

## Query Operations

### SearchAsync

Search projects by name or description.

```csharp
var results = await client.ProjectManagement.Projects.SearchAsync(
    "website",
    take: 10
);
```

### GetSubProjectsAsync

Get child projects of a parent project.

```csharp
var subProjects = await client.ProjectManagement.Projects.GetSubProjectsAsync(
    parentId: 123
);
```

### GetKeysAsync

Get only project IDs matching query (efficient for large datasets).

```csharp
var projectIds = await client.ProjectManagement.Projects.GetKeysAsync(
    new ProjectQueryParams { Status = "active" }
);
```

---

## Models

### ProjectSummary

```csharp
public class ProjectSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal? BudgetHours { get; set; }
    public decimal UsedHours { get; set; }
    public DateTime? DueDate { get; set; }
}
```

### ProjectDetail

```csharp
public class ProjectDetail : ProjectSummary
{
    public string Description { get; set; }
    public DateTime? StartDate { get; set; }
    public int? CategoryId { get; set; }
    public int? ParentProjectId { get; set; }
    public List<int> AssignedEmployeeIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```
