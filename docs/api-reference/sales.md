# Sales API

CRM operations for pipelines, deals, and sales analytics.

Access via: `client.SalesModule`

---

## Pipelines

### GetPipelinesAsync

Get all pipelines for a workspace.

```csharp
var pipelines = await client.SalesModule.GetPipelinesAsync(workspaceId: 1);

foreach (var pipeline in pipelines)
{
    Console.WriteLine($"{pipeline.Name}: {pipeline.DealCount} deals");
}
```

**Returns:** `List<PipelineListViewModel>`

---

### GetPipelineAsync

Get a pipeline with stages.

```csharp
var pipeline = await client.SalesModule.GetPipelineAsync(123);

Console.WriteLine($"Pipeline: {pipeline.Name}");
foreach (var stage in pipeline.Stages)
{
    Console.WriteLine($"  {stage.Name}: {stage.DealCount} deals ({stage.TotalValue:C})");
}
```

---

### CreatePipelineAsync

Create a new sales pipeline.

```csharp
var pipeline = await client.SalesModule.CreatePipelineAsync(
    new CreatePipelineRequest
    {
        WorkspaceId = 1,
        Name = "Enterprise Sales",
        Stages = new List<CreateStageRequest>
        {
            new() { Name = "Lead", Order = 1, Probability = 10 },
            new() { Name = "Discovery", Order = 2, Probability = 25 },
            new() { Name = "Proposal", Order = 3, Probability = 50 },
            new() { Name = "Negotiation", Order = 4, Probability = 75 },
            new() { Name = "Closed Won", Order = 5, Probability = 100, IsWonStage = true },
            new() { Name = "Closed Lost", Order = 6, Probability = 0, IsLostStage = true }
        }
    }
);
```

---

### UpdatePipelineAsync

Update pipeline settings.

```csharp
await client.SalesModule.UpdatePipelineAsync(
    123,
    new UpdatePipelineRequest
    {
        Name = "Enterprise Sales 2026",
        DefaultCurrency = "DKK"
    }
);
```

---

### DeletePipelineAsync

Delete a pipeline.

```csharp
await client.SalesModule.DeletePipelineAsync(123);
```

> **Warning:** This will delete all deals in the pipeline.

---

## Deals

### SearchDealsAsync

Search and filter deals.

```csharp
// All open deals
var deals = await client.SalesModule.SearchDealsAsync(
    new DealSearchRequest
    {
        PipelineId = 123,
        Status = "Open"
    }
);

// Deals for a specific customer
var customerDeals = await client.SalesModule.SearchDealsAsync(
    new DealSearchRequest
    {
        CustomerId = 456
    }
);

// High-value deals
var bigDeals = await client.SalesModule.SearchDealsAsync(
    new DealSearchRequest
    {
        MinValue = 100000,
        Status = "Open"
    }
);
```

**Search Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `PipelineId` | int? | Filter by pipeline |
| `StageId` | int? | Filter by stage |
| `CustomerId` | int? | Filter by customer |
| `OwnerId` | int? | Filter by deal owner |
| `Status` | string? | Open, Won, Lost |
| `MinValue` | decimal? | Minimum deal value |
| `MaxValue` | decimal? | Maximum deal value |
| `Take` | int | Max results (default: 50) |

---

### GetDealAsync

Get deal details.

```csharp
var deal = await client.SalesModule.GetDealAsync(789);

Console.WriteLine($"Deal: {deal.Name}");
Console.WriteLine($"Value: {deal.Value:C}");
Console.WriteLine($"Stage: {deal.StageName}");
Console.WriteLine($"Customer: {deal.CustomerName}");
Console.WriteLine($"Expected Close: {deal.ExpectedCloseDate:d}");
```

---

### CreateDealAsync

Create a new deal.

```csharp
var deal = await client.SalesModule.CreateDealAsync(
    new CreateDealRequest
    {
        PipelineId = 123,
        StageId = 456,
        Name = "Acme Corp - Enterprise License",
        CustomerId = 789,
        Value = 150000,
        Currency = "DKK",
        ExpectedCloseDate = DateTime.Today.AddMonths(2),
        OwnerId = 1,
        Notes = "Initial contact through partner referral"
    }
);

Console.WriteLine($"Created deal #{deal.Id}");
```

---

### UpdateDealAsync

Update deal information.

```csharp
await client.SalesModule.UpdateDealAsync(
    789,
    new UpdateDealRequest
    {
        Value = 175000,
        ExpectedCloseDate = DateTime.Today.AddMonths(1),
        Notes = "Increased scope after discovery meeting"
    }
);
```

---

### DeleteDealAsync

Delete a deal.

```csharp
await client.SalesModule.DeleteDealAsync(789);
```

---

### MoveDealAsync

Move a deal to a different stage.

```csharp
var deal = await client.SalesModule.MoveDealAsync(
    id: 789,
    stageId: 456  // Target stage
);

Console.WriteLine($"Deal moved to: {deal.StageName}");
```

---

### MarkWonAsync

Mark a deal as won.

```csharp
var deal = await client.SalesModule.MarkWonAsync(
    789,
    new WonDealRequest
    {
        ClosedValue = 150000,
        ClosedDate = DateTime.Today,
        Notes = "3-year enterprise agreement signed"
    }
);
```

---

### MarkLostAsync

Mark a deal as lost.

```csharp
var deal = await client.SalesModule.MarkLostAsync(
    789,
    new LostDealRequest
    {
        LostReason = "Price",
        CompetitorName = "Competitor X",
        Notes = "Budget constraints - revisit Q3"
    }
);
```

**Common Lost Reasons:**
- `Price` - Too expensive
- `Competitor` - Chose competitor
- `NoDecision` - No decision made
- `Timing` - Bad timing
- `Features` - Missing features
- `Other` - Other reason

---

### ReopenDealAsync

Reopen a closed deal.

```csharp
var deal = await client.SalesModule.ReopenDealAsync(789);

Console.WriteLine($"Deal reopened in stage: {deal.StageName}");
```

---

## Analytics

### GetPipelineAnalyticsAsync

Get pipeline performance metrics.

```csharp
var analytics = await client.SalesModule.GetPipelineAnalyticsAsync(
    pipelineId: 123,
    fromDate: DateTime.Today.AddMonths(-3),
    toDate: DateTime.Today
);

Console.WriteLine($"Total Value: {analytics.TotalValue:C}");
Console.WriteLine($"Won Value: {analytics.WonValue:C}");
Console.WriteLine($"Win Rate: {analytics.WinRate:P0}");
Console.WriteLine($"Avg Deal Size: {analytics.AverageDealSize:C}");
Console.WriteLine($"Avg Sales Cycle: {analytics.AverageSalesCycleDays} days");

foreach (var stage in analytics.StageMetrics)
{
    Console.WriteLine($"  {stage.StageName}: {stage.DealCount} deals ({stage.TotalValue:C})");
}
```

---

### GetForecastAsync

Get sales forecast.

```csharp
// Monthly forecast
var forecast = await client.SalesModule.GetForecastAsync(
    pipelineId: 123,
    period: "month"
);

Console.WriteLine($"Period: {forecast.PeriodStart:d} - {forecast.PeriodEnd:d}");
Console.WriteLine($"Committed: {forecast.CommittedValue:C}");
Console.WriteLine($"Best Case: {forecast.BestCaseValue:C}");
Console.WriteLine($"Pipeline: {forecast.PipelineValue:C}");
Console.WriteLine($"Weighted Forecast: {forecast.WeightedForecast:C}");
```

**Period Values:**
- `week` - This week
- `month` - This month
- `quarter` - This quarter

---

## Models

### PipelineViewModel

```csharp
public class PipelineViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DefaultCurrency { get; set; }
    public int DealCount { get; set; }
    public decimal TotalValue { get; set; }
    public List<StageViewModel> Stages { get; set; }
}
```

### StageViewModel

```csharp
public class StageViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int Probability { get; set; }
    public bool IsWonStage { get; set; }
    public bool IsLostStage { get; set; }
    public int DealCount { get; set; }
    public decimal TotalValue { get; set; }
}
```

### DealViewModel

```csharp
public class DealViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int PipelineId { get; set; }
    public string PipelineName { get; set; }
    public int StageId { get; set; }
    public string StageName { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int? OwnerId { get; set; }
    public string OwnerName { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string Notes { get; set; }
    public string LostReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## Common Patterns

### Pipeline Overview Dashboard

```csharp
public async Task<PipelineDashboard> GetPipelineDashboardAsync(int pipelineId)
{
    var pipeline = await _client.SalesModule.GetPipelineAsync(pipelineId);
    var analytics = await _client.SalesModule.GetPipelineAnalyticsAsync(pipelineId);
    var forecast = await _client.SalesModule.GetForecastAsync(pipelineId, "month");

    return new PipelineDashboard
    {
        Pipeline = pipeline,
        Analytics = analytics,
        Forecast = forecast,
        StagesWithDeals = pipeline.Stages
            .Where(s => !s.IsWonStage && !s.IsLostStage)
            .OrderBy(s => s.Order)
            .ToList()
    };
}
```

### Deal Aging Report

```csharp
public async Task<List<DealAging>> GetAgingDealsAsync(int pipelineId)
{
    var deals = await _client.SalesModule.SearchDealsAsync(
        new DealSearchRequest
        {
            PipelineId = pipelineId,
            Status = "Open"
        }
    );

    return deals
        .Select(d => new DealAging
        {
            Deal = d,
            DaysInPipeline = (DateTime.Today - d.CreatedAt).Days,
            DaysUntilExpectedClose = d.ExpectedCloseDate.HasValue
                ? (d.ExpectedCloseDate.Value - DateTime.Today).Days
                : (int?)null,
            IsOverdue = d.ExpectedCloseDate.HasValue && d.ExpectedCloseDate.Value < DateTime.Today
        })
        .OrderByDescending(d => d.DaysInPipeline)
        .ToList();
}
```

### Win/Loss Analysis

```csharp
public async Task<WinLossAnalysis> AnalyzeWinLossAsync(int pipelineId, DateTime fromDate)
{
    var wonDeals = await _client.SalesModule.SearchDealsAsync(
        new DealSearchRequest { PipelineId = pipelineId, Status = "Won" }
    );

    var lostDeals = await _client.SalesModule.SearchDealsAsync(
        new DealSearchRequest { PipelineId = pipelineId, Status = "Lost" }
    );

    return new WinLossAnalysis
    {
        WonCount = wonDeals.Count,
        LostCount = lostDeals.Count,
        WinRate = wonDeals.Count / (double)(wonDeals.Count + lostDeals.Count),
        TotalWonValue = wonDeals.Sum(d => d.Value),
        TotalLostValue = lostDeals.Sum(d => d.Value),
        LostReasons = lostDeals
            .GroupBy(d => d.LostReason ?? "Unknown")
            .Select(g => new LostReasonSummary
            {
                Reason = g.Key,
                Count = g.Count(),
                TotalValue = g.Sum(d => d.Value)
            })
            .OrderByDescending(r => r.Count)
            .ToList()
    };
}
```
