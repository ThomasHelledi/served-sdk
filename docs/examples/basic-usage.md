# Basic Usage Examples

Common patterns and examples for using Served.SDK.

---

## Project Workflow

### Create a Complete Project Structure

```csharp
// Create the main project
var project = await client.ProjectManagement.Projects.CreateAsync(
    new CreateProjectRequest
    {
        Name = "E-commerce Platform",
        Description = "Build new e-commerce solution",
        CustomerId = customerId,
        BudgetHours = 500,
        StartDate = DateTime.Today,
        DueDate = DateTime.Today.AddMonths(6)
    }
);

// Create tasks in bulk
var tasks = await client.ProjectManagement.Tasks.CreateBulkAsync(
    new BulkCreateTasksRequest
    {
        ProjectId = project.Id,
        Tasks = new List<CreateTaskRequest>
        {
            new() { Name = "Requirements gathering", EstimatedHours = 20 },
            new() { Name = "UI/UX Design", EstimatedHours = 40 },
            new() { Name = "Frontend development", EstimatedHours = 120 },
            new() { Name = "Backend development", EstimatedHours = 160 },
            new() { Name = "Testing & QA", EstimatedHours = 80 },
            new() { Name = "Deployment", EstimatedHours = 20 }
        }
    }
);

Console.WriteLine($"Created project #{project.Id} with {tasks.Succeeded.Count} tasks");
```

---

## Time Tracking

### Log Daily Work

```csharp
public async Task LogDailyWorkAsync(int taskId, decimal hours, string description)
{
    var entry = await client.Registration.TimeRegistrations.CreateAsync(
        new CreateTimeRegistrationRequest
        {
            TaskId = taskId,
            Date = DateTime.Today,
            Hours = hours,
            Description = description
        }
    );

    Console.WriteLine($"Logged {hours}h on task #{taskId}");
}
```

### Weekly Time Report

```csharp
public async Task<WeeklyReport> GenerateWeeklyReportAsync(DateTime weekStart)
{
    var entries = await client.Registration.TimeRegistrations.GetByDateRangeAsync(
        start: weekStart,
        end: weekStart.AddDays(6)
    );

    return new WeeklyReport
    {
        WeekStart = weekStart,
        TotalHours = entries.Sum(e => e.Hours),
        BillableHours = entries.Where(e => e.Billable).Sum(e => e.Hours),
        ByProject = entries
            .GroupBy(e => e.ProjectName)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours)),
        ByDay = entries
            .GroupBy(e => e.Date.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours))
    };
}
```

---

## Task Management

### Get My Open Tasks

```csharp
public async Task<List<TaskSummary>> GetMyOpenTasksAsync(int employeeId)
{
    return await client.ProjectManagement.Tasks.GetAllAsync(
        new TaskQueryParams
        {
            AssigneeId = employeeId,
            IncludeCompleted = false
        }
    );
}
```

### Complete Multiple Tasks

```csharp
public async Task CompleteTasksAsync(params int[] taskIds)
{
    var result = await client.ProjectManagement.Tasks.UpdateStatusBulkAsync(
        new BulkUpdateTaskStatusRequest
        {
            Updates = taskIds.Select(id => new TaskStatusUpdate
            {
                Id = id,
                Status = "completed",
                CompletedAt = DateTime.UtcNow
            }).ToList()
        }
    );

    Console.WriteLine($"Completed {result.Succeeded.Count} tasks");
}
```

---

## Customer Management

### Create Customer with Project

```csharp
public async Task<(CustomerDetail customer, ProjectDetail project)>
    OnboardCustomerAsync(string name, string email, string projectName)
{
    // Create customer
    var customer = await client.Companies.Customers.CreateAsync(
        new CreateCustomerRequest
        {
            Name = name,
            Email = email
        }
    );

    // Create initial project
    var project = await client.ProjectManagement.Projects.CreateAsync(
        new CreateProjectRequest
        {
            Name = projectName,
            CustomerId = customer.Id,
            StartDate = DateTime.Today
        }
    );

    return (customer, project);
}
```

---

## Dashboard & Reporting

### Project Status Overview

```csharp
public async Task<ProjectOverview> GetProjectOverviewAsync(int projectId)
{
    var project = await client.ProjectManagement.Projects.GetAsync(projectId);
    var tasks = await client.ProjectManagement.Tasks.GetByProjectAsync(projectId);
    var time = await client.Registration.TimeRegistrations.GetByProjectAsync(projectId);

    var completedTasks = tasks.Count(t => t.Status == "completed");
    var totalTasks = tasks.Count;

    return new ProjectOverview
    {
        Project = project,
        TaskProgress = totalTasks > 0 ? (decimal)completedTasks / totalTasks : 0,
        CompletedTasks = completedTasks,
        TotalTasks = totalTasks,
        HoursUsed = time.Sum(t => t.Hours),
        HoursRemaining = (project.BudgetHours ?? 0) - time.Sum(t => t.Hours),
        BudgetUtilization = project.BudgetHours > 0
            ? time.Sum(t => t.Hours) / project.BudgetHours.Value
            : 0
    };
}
```

---

## Error Handling Pattern

### Robust API Calls

```csharp
public async Task<T?> SafeApiCallAsync<T>(Func<Task<T>> apiCall) where T : class
{
    try
    {
        return await apiCall();
    }
    catch (ServedApiException ex) when (ex.StatusCode == 404)
    {
        _logger.LogWarning("Resource not found: {Message}", ex.Message);
        return null;
    }
    catch (ServedApiException ex) when (ex.StatusCode == 429)
    {
        _logger.LogWarning("Rate limited, retrying after delay...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        return await apiCall();
    }
    catch (ServedApiException ex)
    {
        _logger.LogError(ex, "API error: {StatusCode} - {Message}",
            ex.StatusCode, ex.Message);
        throw;
    }
}

// Usage
var project = await SafeApiCallAsync(() =>
    client.ProjectManagement.Projects.GetAsync(123));
```

---

## Integration Patterns

### Sync with External System

```csharp
public async Task SyncProjectsToExternalAsync(IExternalSystem external)
{
    var projects = await client.ProjectManagement.Projects.GetAllAsync(
        new ProjectQueryParams { Status = "active" }
    );

    foreach (var project in projects)
    {
        var tasks = await client.ProjectManagement.Tasks.GetByProjectAsync(
            project.Id,
            includeCompleted: true
        );

        await external.UpsertProjectAsync(new ExternalProject
        {
            ExternalId = $"served-{project.Id}",
            Name = project.Name,
            TaskCount = tasks.Count,
            CompletedCount = tasks.Count(t => t.Status == "completed"),
            TotalHours = tasks.Sum(t => t.LoggedHours)
        });
    }
}
```
