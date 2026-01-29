# Served.SDK.Testing

Testing utilities for the Served platform with Playwright-based browser automation and AI-driven test flows.

## Installation

```bash
dotnet add package Served.SDK.Testing
```

## Quick Start

```csharp
using Served.SDK.Testing;
using Served.SDK.Testing.Configuration;

// Create test client with default configuration
await using var client = new ServedTestClient(TestConfiguration.Default);
await client.InitializeAsync();

// Navigate and verify version
await client.NavigateToUnifiedHQAsync();
var version = await client.GetVersionFromFooterAsync();
Console.WriteLine($"Version: {version}");

// Take screenshot
await client.ScreenshotAsync("homepage");
```

## Configuration

```csharp
var config = new TestConfiguration
{
    BaseUrl = "https://app.served.dk",
    ApiUrl = "https://api.served.dk",
    UnifiedHQUrl = "https://unifiedhq.ai",
    BrowserType = BrowserType.Chromium,
    Headless = true,
    TrackEvents = true,
    Credentials = new TestCredentials
    {
        Username = "test@example.com",
        Password = "password"
    }
};

await using var client = new ServedTestClient(config);
```

### Environment Variables

```bash
SERVED_BASE_URL=https://app.served.dk
SERVED_API_URL=https://api.served.dk
SERVED_API_KEY=your-api-key
TEST_HEADLESS=true
TEST_BROWSER=Chromium
```

## Authentication

```csharp
// Login with configured credentials
await client.Auth.LoginAsync();

// Or specify credentials
await client.Auth.LoginAsync("user@example.com", "password");

// Check login status
var isLoggedIn = await client.Auth.IsLoggedInAsync();

// Logout
await client.Auth.LogoutAsync();
```

## Task Operations

```csharp
// Navigate to tasks
await client.Tasks.NavigateToTasksAsync();

// Search for a task
await client.Tasks.SearchTaskAsync("My Task");

// Select task by title
await client.Tasks.SelectTaskByTitleAsync("Important Task");

// Add a comment
await client.Tasks.AddCommentAsync("Looking at this task now");

// Change status
await client.Tasks.ChangeStatusAsync("In Progress");
```

## Event Tracking

```csharp
// Events are tracked automatically when TrackEvents = true
await client.NavigateToAppAsync();
await client.ClickAsync("button.submit");

// Get metrics
var metrics = client.Events.GetMetrics();
Console.WriteLine($"Total events: {metrics.TotalEvents}");
Console.WriteLine($"Page loads: {metrics.PageLoadCount}");
Console.WriteLine($"Clicks: {metrics.ClickCount}");

// Export events to JSON
await client.Events.SaveToFileAsync("./test-results/events.json");
```

## AI-Driven Test Flows

```csharp
using Served.SDK.Testing.AI;

var orchestrator = new AITestOrchestrator(client);

var flow = new AITestFlow
{
    Name = "Login and Find Task",
    Steps = new List<AITestStep>
    {
        new() { Action = AIAction.Navigate, Target = "https://app.served.dk/login" },
        new() { Action = AIAction.Fill, Target = "[name='email']", Value = "user@example.com" },
        new() { Action = AIAction.Fill, Target = "[name='password']", Value = "password" },
        new() { Action = AIAction.Click, Target = "button[type='submit']" },
        new() { Action = AIAction.Wait, Target = "[data-testid='dashboard']" },
        new()
        {
            Action = AIAction.Decision,
            DecisionContext = "Find the most relevant task",
            DecisionOptions = new() { "recent", "urgent", "assigned" }
        },
        new() { Action = AIAction.Click, Target = ".task-item:first-child" },
        new()
        {
            Action = AIAction.Fill,
            Target = "[data-testid='comment-input']",
            Value = "Looking at this task now"
        },
        new() { Action = AIAction.Click, Target = "button:has-text('Add Comment')" }
    }
};

var result = await orchestrator.ExecuteFlowAsync(flow);
Console.WriteLine($"Flow: {result.FlowName}, Status: {result.Status}");

foreach (var decision in result.Decisions)
{
    Console.WriteLine($"Decision: {decision.SelectedOption} - {decision.Reasoning}");
}
```

## Version Verification Test

```csharp
// Test 1: Verify version in footer
await client.NavigateAsync("https://apis.unifiedhq.ai");
await client.WaitForReadyAsync();

var version = await client.GetVersionFromFooterAsync();
Assert.NotNull(version);
Assert.True(version.StartsWith("v2026"));

// Verify via API
var response = await httpClient.GetAsync("https://apis.unifiedhq.ai/api/version");
var versionInfo = await response.Content.ReadFromJsonAsync<VersionInfo>();
Assert.Equal("healthy", versionInfo.Components["hub"].Status);
```

## Test Results & Screenshots

```csharp
// Screenshots saved automatically on failure if ScreenshotOnFailure = true
config.ScreenshotOnFailure = true;
config.ScreenshotDir = "./test-results/screenshots";
config.VideoDir = "./test-results/videos";
config.RecordVideo = true;
```

## Integration with xUnit

```csharp
public class ServedTests : IAsyncLifetime
{
    private ServedTestClient _client = null!;

    public async Task InitializeAsync()
    {
        _client = new ServedTestClient(TestConfiguration.FromEnvironment());
        await _client.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _client.DisposeAsync();
    }

    [Fact]
    public async Task VersionPage_DisplaysVersion()
    {
        await _client.NavigateToUnifiedHQAsync();
        var version = await _client.GetVersionFromFooterAsync();
        Assert.NotNull(version);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Succeeds()
    {
        var success = await _client.Auth.LoginAsync("test@example.com", "password");
        Assert.True(success);
    }
}
```

## License

MIT License - See LICENSE for details.
