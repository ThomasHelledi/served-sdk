# Tasks API

Manage tasks within projects.

Access via: `client.ProjectManagement.Tasks`

---

## Methods

### GetAllAsync

Get all tasks with optional filtering.

```csharp
var tasks = await client.ProjectManagement.Tasks.GetAllAsync();

// With filtering
var myTasks = await client.ProjectManagement.Tasks.GetAllAsync(
    new TaskQueryParams
    {
        ProjectId = 123,
        AssigneeId = 456,
        Status = "in_progress",
        IncludeCompleted = false
    }
);
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `ProjectId` | int? | Filter by project |
| `AssigneeId` | int? | Filter by assigned employee |
| `Status` | string | Filter by status |
| `Priority` | string | Filter by priority (low, medium, high, critical) |
| `IncludeCompleted` | bool | Include completed tasks (default: true) |
| `Take` | int | Max results |
| `Skip` | int | Offset for pagination |

**Returns:** `List<TaskSummary>`

---

### GetAsync

Get a single task by ID.

```csharp
var task = await client.ProjectManagement.Tasks.GetAsync(456);

Console.WriteLine($"Task: {task.Name}");
Console.WriteLine($"Status: {task.Status}");
Console.WriteLine($"Estimate: {task.EstimatedHours}h");
Console.WriteLine($"Logged: {task.LoggedHours}h");
```

**Returns:** `TaskDetail`

---

### CreateAsync

Create a new task.

```csharp
var task = await client.ProjectManagement.Tasks.CreateAsync(
    new CreateTaskRequest
    {
        ProjectId = 123,
        Name = "Implement login page",
        Description = "Create login form with validation",
        AssigneeId = 789,
        Priority = "high",
        EstimatedHours = 8,
        DueDate = DateTime.Today.AddDays(7)
    }
);

Console.WriteLine($"Created task #{task.Id}");
```

**Request Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `ProjectId` | int | Yes | Parent project ID |
| `Name` | string | Yes | Task name |
| `Description` | string | No | Task description |
| `AssigneeId` | int | No | Assigned employee ID |
| `Priority` | string | No | Priority level |
| `EstimatedHours` | decimal | No | Hour estimate |
| `DueDate` | DateTime | No | Due date |
| `ParentTaskId` | int | No | Parent task (for subtasks) |

---

### UpdateAsync

Update an existing task.

```csharp
var updated = await client.ProjectManagement.Tasks.UpdateAsync(
    456,
    new UpdateTaskRequest
    {
        Name = "Implement login page with OAuth",
        EstimatedHours = 12,
        Priority = "critical"
    }
);
```

---

### UpdateStatusAsync

Update only the task status (optimized endpoint).

```csharp
var updated = await client.ProjectManagement.Tasks.UpdateStatusAsync(
    456,
    new UpdateTaskStatusRequest
    {
        Status = "completed",
        CompletedAt = DateTime.UtcNow
    }
);
```

**Status Values:**

| Status | Description |
|--------|-------------|
| `todo` | Not started |
| `in_progress` | Currently being worked on |
| `review` | Pending review |
| `completed` | Done |
| `blocked` | Blocked by dependency |

---

### DeleteAsync

Delete a task.

```csharp
await client.ProjectManagement.Tasks.DeleteAsync(456);
```

---

## Bulk Operations

### CreateBulkAsync

Create multiple tasks at once.

```csharp
var result = await client.ProjectManagement.Tasks.CreateBulkAsync(
    new BulkCreateTasksRequest
    {
        ProjectId = 123,
        Tasks = new List<CreateTaskRequest>
        {
            new() { Name = "Design mockups", EstimatedHours = 4 },
            new() { Name = "Frontend implementation", EstimatedHours = 16 },
            new() { Name = "Backend API", EstimatedHours = 12 },
            new() { Name = "Testing", EstimatedHours = 8 }
        }
    }
);

Console.WriteLine($"Created {result.Succeeded.Count} tasks");
```

### UpdateStatusBulkAsync

Update status for multiple tasks.

```csharp
var result = await client.ProjectManagement.Tasks.UpdateStatusBulkAsync(
    new BulkUpdateTaskStatusRequest
    {
        Updates = new List<TaskStatusUpdate>
        {
            new() { Id = 1, Status = "completed" },
            new() { Id = 2, Status = "completed" },
            new() { Id = 3, Status = "in_progress" }
        }
    }
);
```

---

## Query Operations

### GetByProjectAsync

Get all tasks for a specific project.

```csharp
var projectTasks = await client.ProjectManagement.Tasks.GetByProjectAsync(
    projectId: 123,
    includeCompleted: false
);
```

### GetByAssigneeAsync

Get tasks assigned to a specific employee.

```csharp
var myTasks = await client.ProjectManagement.Tasks.GetByAssigneeAsync(
    employeeId: 789,
    includeCompleted: false
);
```

### GetSubTasksAsync

Get child tasks of a parent task.

```csharp
var subtasks = await client.ProjectManagement.Tasks.GetSubTasksAsync(
    parentTaskId: 456
);
```

### SearchAsync

Search tasks by name or description.

```csharp
var results = await client.ProjectManagement.Tasks.SearchAsync(
    "login",
    take: 20
);
```

---

## Models

### TaskSummary

```csharp
public class TaskSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public int? AssigneeId { get; set; }
    public string AssigneeName { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal LoggedHours { get; set; }
    public DateTime? DueDate { get; set; }
}
```

### TaskDetail

```csharp
public class TaskDetail : TaskSummary
{
    public string Description { get; set; }
    public int? ParentTaskId { get; set; }
    public int SubTaskCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<int> DependencyIds { get; set; }
}
```
