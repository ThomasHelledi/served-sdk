using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Served.SDK.Client;
using Served.SDK.Testing.Configuration;
using Served.SDK.Testing.Helpers;
using Served.SDK.Testing.Actions;
using Served.SDK.Testing.Browser;

namespace Served.SDK.Testing;

/// <summary>
/// Main client for Served platform testing with Playwright integration.
/// Provides browser automation, AI-driven flows, and testing utilities.
/// </summary>
public class ServedTestClient : IAsyncDisposable
{
    private readonly TestConfiguration _config;
    private readonly ILogger<ServedTestClient>? _logger;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    /// <summary>
    /// Gets the current Playwright page instance.
    /// </summary>
    public IPage? Page => _page;

    /// <summary>
    /// Gets the current browser context.
    /// </summary>
    public IBrowserContext? Context => _context;

    /// <summary>
    /// Gets the Served SDK client for API interactions.
    /// </summary>
    public ServedClient? ServedClient { get; private set; }

    /// <summary>
    /// Gets the authentication helper for login flows.
    /// </summary>
    public AuthHelper Auth { get; }

    /// <summary>
    /// Gets the task actions helper for task-related operations.
    /// </summary>
    public TaskActions Tasks { get; }

    /// <summary>
    /// Gets the browser event tracker.
    /// </summary>
    public BrowserEventTracker Events { get; }

    /// <summary>
    /// Gets the test configuration.
    /// </summary>
    public TestConfiguration Config => _config;

    /// <summary>
    /// Creates a new Served test client with the specified configuration.
    /// </summary>
    public ServedTestClient(TestConfiguration config, ILogger<ServedTestClient>? logger = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger;

        Auth = new AuthHelper(this);
        Tasks = new TaskActions(this);
        Events = new BrowserEventTracker(this);
    }

    /// <summary>
    /// Creates a new Served test client with default configuration.
    /// </summary>
    public ServedTestClient() : this(TestConfiguration.Default)
    {
    }

    /// <summary>
    /// Initializes the browser and Playwright instance.
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger?.LogInformation("Initializing Playwright...");

        _playwright = await Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _config.Headless,
            SlowMo = _config.SlowMo,
            Timeout = _config.Timeout
        };

        _browser = _config.BrowserType switch
        {
            Configuration.BrowserType.Firefox => await _playwright.Firefox.LaunchAsync(launchOptions),
            Configuration.BrowserType.Webkit => await _playwright.Webkit.LaunchAsync(launchOptions),
            _ => await _playwright.Chromium.LaunchAsync(launchOptions)
        };

        var contextOptions = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = _config.ViewportWidth,
                Height = _config.ViewportHeight
            },
            RecordVideoDir = _config.RecordVideo ? _config.VideoDir : null,
            BaseURL = _config.BaseUrl
        };

        _context = await _browser.NewContextAsync(contextOptions);
        _page = await _context.NewPageAsync();

        // Initialize SDK client if API key provided
        if (!string.IsNullOrEmpty(_config.ApiKey))
        {
            ServedClient = new ServedClient(_config.ApiUrl, _config.ApiKey);
        }

        // Start event tracking if enabled
        if (_config.TrackEvents)
        {
            await Events.StartTrackingAsync();
        }

        _logger?.LogInformation("Playwright initialized successfully");
    }

    /// <summary>
    /// Navigates to the specified URL.
    /// </summary>
    public async Task<IResponse?> NavigateAsync(string url)
    {
        EnsureInitialized();
        _logger?.LogInformation("Navigating to {Url}", url);

        var response = await _page!.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = _config.NavigationTimeout
        });

        Events.TrackPageLoad(url);
        return response;
    }

    /// <summary>
    /// Navigates to the Served app.
    /// </summary>
    public Task<IResponse?> NavigateToAppAsync() => NavigateAsync(_config.BaseUrl);

    /// <summary>
    /// Navigates to the UnifiedHQ platform.
    /// </summary>
    public Task<IResponse?> NavigateToUnifiedHQAsync() => NavigateAsync(_config.UnifiedHQUrl);

    /// <summary>
    /// Takes a screenshot and saves it to the configured directory.
    /// </summary>
    public async Task<string> ScreenshotAsync(string name)
    {
        EnsureInitialized();

        var path = Path.Combine(_config.ScreenshotDir, $"{name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png");
        Directory.CreateDirectory(_config.ScreenshotDir);

        await _page!.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });

        _logger?.LogInformation("Screenshot saved to {Path}", path);
        return path;
    }

    /// <summary>
    /// Waits for the page to be ready (network idle + DOM loaded).
    /// </summary>
    public async Task WaitForReadyAsync()
    {
        EnsureInitialized();
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Gets the current page title.
    /// </summary>
    public async Task<string> GetTitleAsync()
    {
        EnsureInitialized();
        return await _page!.TitleAsync();
    }

    /// <summary>
    /// Gets the current page URL.
    /// </summary>
    public string GetUrl()
    {
        EnsureInitialized();
        return _page!.Url;
    }

    /// <summary>
    /// Clicks an element by selector.
    /// </summary>
    public async Task ClickAsync(string selector)
    {
        EnsureInitialized();
        _logger?.LogDebug("Clicking {Selector}", selector);

        await _page!.ClickAsync(selector, new PageClickOptions
        {
            Timeout = _config.Timeout
        });

        Events.TrackClick(selector);
    }

    /// <summary>
    /// Fills a form field by selector.
    /// </summary>
    public async Task FillAsync(string selector, string value)
    {
        EnsureInitialized();
        _logger?.LogDebug("Filling {Selector} with value", selector);

        await _page!.FillAsync(selector, value, new PageFillOptions
        {
            Timeout = _config.Timeout
        });
    }

    /// <summary>
    /// Gets the text content of an element.
    /// </summary>
    public async Task<string?> GetTextAsync(string selector)
    {
        EnsureInitialized();
        return await _page!.TextContentAsync(selector);
    }

    /// <summary>
    /// Checks if an element is visible.
    /// </summary>
    public async Task<bool> IsVisibleAsync(string selector)
    {
        EnsureInitialized();
        return await _page!.IsVisibleAsync(selector);
    }

    /// <summary>
    /// Waits for an element to be visible.
    /// </summary>
    public async Task WaitForSelectorAsync(string selector)
    {
        EnsureInitialized();
        await _page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _config.Timeout
        });
    }

    /// <summary>
    /// Gets the version information from the footer.
    /// </summary>
    public async Task<string?> GetVersionFromFooterAsync()
    {
        EnsureInitialized();

        // Look for version in footer (common patterns)
        var selectors = new[]
        {
            "[data-testid='version']",
            ".version",
            "footer .font-mono",
            "footer span:has-text('v')"
        };

        foreach (var selector in selectors)
        {
            try
            {
                var element = await _page!.QuerySelectorAsync(selector);
                if (element != null)
                {
                    var text = await element.TextContentAsync();
                    if (!string.IsNullOrEmpty(text) && text.Contains('v'))
                    {
                        return text.Trim();
                    }
                }
            }
            catch
            {
                // Try next selector
            }
        }

        return null;
    }

    /// <summary>
    /// Verifies that the page displays the expected version.
    /// </summary>
    public async Task<bool> VerifyVersionAsync(string expectedVersion)
    {
        var actualVersion = await GetVersionFromFooterAsync();

        if (actualVersion == null)
        {
            _logger?.LogWarning("Could not find version in footer");
            return false;
        }

        var matches = actualVersion.Contains(expectedVersion);
        _logger?.LogInformation("Version check: expected={Expected}, actual={Actual}, matches={Matches}",
            expectedVersion, actualVersion, matches);

        return matches;
    }

    /// <summary>
    /// Ensures the client has been initialized.
    /// </summary>
    private void EnsureInitialized()
    {
        if (_page == null)
        {
            throw new InvalidOperationException("Test client not initialized. Call InitializeAsync() first.");
        }
    }

    /// <summary>
    /// Disposes of the browser and Playwright resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _logger?.LogInformation("Disposing Playwright resources...");

        Events.StopTracking();

        if (_context != null)
        {
            await _context.CloseAsync();
            _context = null;
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
        _page = null;

        _logger?.LogInformation("Playwright resources disposed");
    }
}
