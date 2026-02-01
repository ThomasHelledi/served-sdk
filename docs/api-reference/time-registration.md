# Time Registration API

Track time spent on projects and tasks.

Access via: `client.Registration.TimeRegistrations`

---

## Methods

### GetAllAsync

Get all time registrations with optional filtering.

```csharp
var entries = await client.Registration.TimeRegistrations.GetAllAsync();

// With filtering
var thisWeek = await client.Registration.TimeRegistrations.GetAllAsync(
    new TimeRegistrationQueryParams
    {
        StartDate = DateTime.Today.AddDays(-7),
        EndDate = DateTime.Today,
        EmployeeId = 123
    }
);
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `StartDate` | DateTime? | Filter from date |
| `EndDate` | DateTime? | Filter to date |
| `ProjectId` | int? | Filter by project |
| `TaskId` | int? | Filter by task |
| `EmployeeId` | int? | Filter by employee |
| `Take` | int | Max results (default: 100) |
| `Skip` | int | Offset for pagination |

**Returns:** `List<TimeRegistrationDetail>`

---

### GetAsync

Get a single time registration by ID.

```csharp
var entry = await client.Registration.TimeRegistrations.GetAsync(789);

Console.WriteLine($"Date: {entry.Date:d}");
Console.WriteLine($"Hours: {entry.Hours}");
Console.WriteLine($"Task: {entry.TaskName}");
Console.WriteLine($"Description: {entry.Description}");
```

**Returns:** `TimeRegistrationDetail`

---

### CreateAsync

Log a new time entry.

```csharp
var entry = await client.Registration.TimeRegistrations.CreateAsync(
    new CreateTimeRegistrationRequest
    {
        TaskId = 456,
        Date = DateTime.Today,
        Hours = 2.5m,
        Description = "Implemented login form validation"
    }
);

Console.WriteLine($"Logged {entry.Hours}h on {entry.Date:d}");
```

**Request Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `TaskId` | int | Yes* | Task to log time on |
| `ProjectId` | int | Yes* | Project (if no task) |
| `Date` | DateTime | Yes | Date of work |
| `Hours` | decimal | Yes | Hours worked |
| `Description` | string | No | Work description |
| `Billable` | bool | No | Is billable (default: true) |

*Either `TaskId` or `ProjectId` is required.

---

### UpdateAsync

Update an existing time entry.

```csharp
var updated = await client.Registration.TimeRegistrations.UpdateAsync(
    789,
    new UpdateTimeRegistrationRequest
    {
        Hours = 3.0m,
        Description = "Implemented login form with OAuth support"
    }
);
```

---

### DeleteAsync

Delete a time entry.

```csharp
await client.Registration.TimeRegistrations.DeleteAsync(789);
```

---

## Query Operations

### GetByDateRangeAsync

Get time entries for a date range.

```csharp
var thisMonth = await client.Registration.TimeRegistrations.GetByDateRangeAsync(
    start: new DateTime(2026, 1, 1),
    end: new DateTime(2026, 1, 31),
    take: 200
);

var totalHours = thisMonth.Sum(e => e.Hours);
Console.WriteLine($"Total hours this month: {totalHours}");
```

### GetByProjectAsync

Get time entries for a specific project.

```csharp
var projectTime = await client.Registration.TimeRegistrations.GetByProjectAsync(
    projectId: 123,
    take: 100
);

var billableHours = projectTime.Where(e => e.Billable).Sum(e => e.Hours);
Console.WriteLine($"Billable hours: {billableHours}");
```

### GetByTaskAsync

Get time entries for a specific task.

```csharp
var taskTime = await client.Registration.TimeRegistrations.GetByTaskAsync(
    taskId: 456,
    take: 50
);

Console.WriteLine($"Total time on task: {taskTime.Sum(e => e.Hours)}h");
```

---

## Common Patterns

### Weekly Timesheet

```csharp
public async Task<Dictionary<DateTime, decimal>> GetWeeklyTimesheetAsync(
    int employeeId, DateTime weekStart)
{
    var entries = await _client.Registration.TimeRegistrations.GetByDateRangeAsync(
        start: weekStart,
        end: weekStart.AddDays(6)
    );

    return entries
        .Where(e => e.EmployeeId == employeeId)
        .GroupBy(e => e.Date.Date)
        .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours));
}
```

### Project Time Summary

```csharp
public async Task<ProjectTimeSummary> GetProjectTimeSummaryAsync(int projectId)
{
    var entries = await _client.Registration.TimeRegistrations.GetByProjectAsync(projectId);

    return new ProjectTimeSummary
    {
        TotalHours = entries.Sum(e => e.Hours),
        BillableHours = entries.Where(e => e.Billable).Sum(e => e.Hours),
        ByEmployee = entries
            .GroupBy(e => e.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours))
    };
}
```

---

## Models

### TimeRegistrationDetail

```csharp
public class TimeRegistrationDetail
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string Description { get; set; }
    public bool Billable { get; set; }

    public int? TaskId { get; set; }
    public string TaskName { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### TimeRegistrationQueryParams

```csharp
public class TimeRegistrationQueryParams
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ProjectId { get; set; }
    public int? TaskId { get; set; }
    public int? EmployeeId { get; set; }
    public bool? Billable { get; set; }
    public int Take { get; set; } = 100;
    public int Skip { get; set; } = 0;
}
```
