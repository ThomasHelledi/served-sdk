namespace Served.SDK.Testing.Configuration;

/// <summary>
/// Browser type for Playwright testing.
/// </summary>
public enum BrowserType
{
    Chromium,
    Firefox,
    Webkit
}

/// <summary>
/// Environment for test execution.
/// </summary>
public enum TestEnvironment
{
    Local,
    Staging,
    Production
}

/// <summary>
/// Configuration for Served test client.
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// Base URL for Served app (default: https://app.served.dk).
    /// </summary>
    public string BaseUrl { get; set; } = "https://app.served.dk";

    /// <summary>
    /// API URL for Served API (default: https://api.served.dk).
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.served.dk";

    /// <summary>
    /// UnifiedHQ platform URL (default: https://unifiedhq.ai).
    /// </summary>
    public string UnifiedHQUrl { get; set; } = "https://unifiedhq.ai";

    /// <summary>
    /// UnifiedHQ APIs URL (default: https://apis.unifiedhq.ai).
    /// </summary>
    public string UnifiedHQApisUrl { get; set; } = "https://apis.unifiedhq.ai";

    /// <summary>
    /// API key for authenticated tests.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Browser type to use (default: Chromium).
    /// </summary>
    public BrowserType BrowserType { get; set; } = BrowserType.Chromium;

    /// <summary>
    /// Run browser in headless mode (default: true).
    /// </summary>
    public bool Headless { get; set; } = true;

    /// <summary>
    /// Slow down operations by this amount in milliseconds (default: 0).
    /// </summary>
    public float SlowMo { get; set; } = 0;

    /// <summary>
    /// Default timeout for operations in milliseconds (default: 30000).
    /// </summary>
    public float Timeout { get; set; } = 30000;

    /// <summary>
    /// Navigation timeout in milliseconds (default: 60000).
    /// </summary>
    public float NavigationTimeout { get; set; } = 60000;

    /// <summary>
    /// Viewport width (default: 1920).
    /// </summary>
    public int ViewportWidth { get; set; } = 1920;

    /// <summary>
    /// Viewport height (default: 1080).
    /// </summary>
    public int ViewportHeight { get; set; } = 1080;

    /// <summary>
    /// Whether to record video (default: false).
    /// </summary>
    public bool RecordVideo { get; set; } = false;

    /// <summary>
    /// Directory for video recordings.
    /// </summary>
    public string VideoDir { get; set; } = "./test-results/videos";

    /// <summary>
    /// Directory for screenshots.
    /// </summary>
    public string ScreenshotDir { get; set; } = "./test-results/screenshots";

    /// <summary>
    /// Whether to track browser events (default: true).
    /// </summary>
    public bool TrackEvents { get; set; } = true;

    /// <summary>
    /// WebSocket URL for event streaming.
    /// </summary>
    public string EventsWebSocketUrl { get; set; } = "wss://apis.unifiedhq.ai/api/events/ws";

    /// <summary>
    /// Test environment (default: Production).
    /// </summary>
    public TestEnvironment Environment { get; set; } = TestEnvironment.Production;

    /// <summary>
    /// Number of retries for flaky tests (default: 2).
    /// </summary>
    public int Retries { get; set; } = 2;

    /// <summary>
    /// Whether to take screenshots on failure (default: true).
    /// </summary>
    public bool ScreenshotOnFailure { get; set; } = true;

    /// <summary>
    /// User credentials for authentication tests.
    /// </summary>
    public TestCredentials? Credentials { get; set; }

    /// <summary>
    /// Gets the default configuration for production testing.
    /// </summary>
    public static TestConfiguration Default => new();

    /// <summary>
    /// Gets configuration for local development testing.
    /// </summary>
    public static TestConfiguration Local => new()
    {
        BaseUrl = "http://localhost:5010",
        ApiUrl = "http://localhost:5010",
        UnifiedHQUrl = "http://localhost:4000",
        UnifiedHQApisUrl = "http://localhost:4000",
        Environment = TestEnvironment.Local,
        Headless = false
    };

    /// <summary>
    /// Gets configuration for staging testing.
    /// </summary>
    public static TestConfiguration Staging => new()
    {
        BaseUrl = "https://staging.app.served.dk",
        ApiUrl = "https://staging.api.served.dk",
        Environment = TestEnvironment.Staging
    };

    /// <summary>
    /// Creates configuration from environment variables.
    /// </summary>
    public static TestConfiguration FromEnvironment()
    {
        var config = new TestConfiguration();

        if (System.Environment.GetEnvironmentVariable("SERVED_BASE_URL") is string baseUrl)
            config.BaseUrl = baseUrl;

        if (System.Environment.GetEnvironmentVariable("SERVED_API_URL") is string apiUrl)
            config.ApiUrl = apiUrl;

        if (System.Environment.GetEnvironmentVariable("SERVED_API_KEY") is string apiKey)
            config.ApiKey = apiKey;

        if (System.Environment.GetEnvironmentVariable("UNIFIEDHQ_URL") is string unifiedUrl)
            config.UnifiedHQUrl = unifiedUrl;

        if (System.Environment.GetEnvironmentVariable("TEST_HEADLESS") is string headless)
            config.Headless = bool.Parse(headless);

        if (System.Environment.GetEnvironmentVariable("TEST_BROWSER") is string browser)
            config.BrowserType = Enum.Parse<BrowserType>(browser, true);

        return config;
    }
}

/// <summary>
/// Credentials for authentication tests.
/// </summary>
public class TestCredentials
{
    /// <summary>
    /// Username or email for login.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for login.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Tenant ID for multi-tenant authentication.
    /// </summary>
    public int? TenantId { get; set; }
}
