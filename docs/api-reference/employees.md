# Employees API

Access employee/team member information in your workspace.

Access via: `client.Identity.Employees`

---

## Methods

### ListAsync

List employees with optional search.

```csharp
var employees = await client.Identity.Employees.ListAsync();

// With search
var developers = await client.Identity.Employees.ListAsync(
    search: "developer",
    limit: 20
);

foreach (var emp in employees)
{
    Console.WriteLine($"{emp.Name} - {emp.Email}");
}
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `search` | string | Optional search term |
| `limit` | int | Max results (default: 50) |

**Returns:** `List<EmployeeListViewModel>`

---

### GetDetailedAsync

Get detailed employee information.

```csharp
var employee = await client.Identity.Employees.GetDetailedAsync(userId: 123);

Console.WriteLine($"Name: {employee.Name}");
Console.WriteLine($"Email: {employee.Email}");
Console.WriteLine($"Role: {employee.Role}");
Console.WriteLine($"Department: {employee.Department}");
```

**Returns:** `DetailedEmployeeModel`

---

## Models

### EmployeeListViewModel

```csharp
public class EmployeeListViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
}
```

### DetailedEmployeeModel

```csharp
public class DetailedEmployeeModel : EmployeeListViewModel
{
    public string Department { get; set; }
    public string Title { get; set; }
    public string Phone { get; set; }
    public DateTime? StartDate { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? WeeklyHours { get; set; }
    public List<string> Skills { get; set; }
}
```

---

## Common Patterns

### Team Member Dropdown

```csharp
public async Task<List<SelectOption>> GetTeamMembersAsync()
{
    var employees = await _client.Identity.Employees.ListAsync();

    return employees
        .Where(e => e.IsActive)
        .Select(e => new SelectOption
        {
            Value = e.UserId.ToString(),
            Label = e.Name
        })
        .ToList();
}
```

### Find Tasks by Employee

```csharp
public async Task<EmployeeWorkload> GetEmployeeWorkloadAsync(int userId)
{
    var employee = await _client.Identity.Employees.GetDetailedAsync(userId);
    var tasks = await _client.ProjectManagement.Tasks.GetByAssigneeAsync(
        employeeId: userId,
        includeCompleted: false
    );

    return new EmployeeWorkload
    {
        Employee = employee,
        OpenTasks = tasks.Count,
        TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours ?? 0)
    };
}
```

### Team Utilization Report

```csharp
public async Task<List<TeamUtilization>> GetTeamUtilizationAsync(
    DateTime weekStart)
{
    var employees = await _client.Identity.Employees.ListAsync();
    var results = new List<TeamUtilization>();

    foreach (var emp in employees.Where(e => e.IsActive))
    {
        var timeEntries = await _client.Registration.TimeRegistrations
            .GetByDateRangeAsync(
                weekStart,
                weekStart.AddDays(6)
            );

        var empHours = timeEntries
            .Where(t => t.EmployeeId == emp.UserId)
            .Sum(t => t.Hours);

        results.Add(new TeamUtilization
        {
            EmployeeId = emp.UserId,
            EmployeeName = emp.Name,
            HoursLogged = empHours,
            Utilization = emp.WeeklyHours > 0
                ? empHours / emp.WeeklyHours.Value
                : 0
        });
    }

    return results;
}
```
