using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace Served.SDK.Testing.Browser;

/// <summary>
/// Tracks browser events during test execution.
/// </summary>
public class BrowserEventTracker
{
    private readonly ServedTestClient _client;
    private readonly ConcurrentQueue<BrowserEvent> _events = new();
    private readonly Stopwatch _stopwatch = new();
    private bool _isTracking;
    private string? _sessionId;

    public BrowserEventTracker(ServedTestClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Gets all recorded events.
    /// </summary>
    public IReadOnlyList<BrowserEvent> Events => _events.ToArray();

    /// <summary>
    /// Gets the current session ID.
    /// </summary>
    public string SessionId => _sessionId ?? "unknown";

    /// <summary>
    /// Gets whether tracking is active.
    /// </summary>
    public bool IsTracking => _isTracking;

    /// <summary>
    /// Starts tracking browser events.
    /// </summary>
    public async Task StartTrackingAsync()
    {
        _sessionId = $"test_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
        _isTracking = true;
        _stopwatch.Start();

        // Inject event tracking script into page
        if (_client.Page != null)
        {
            await _client.Page.AddInitScriptAsync(@"
                window.__servedTestEvents = [];
                window.__servedTrackEvent = function(type, data) {
                    window.__servedTestEvents.push({
                        type: type,
                        timestamp: Date.now(),
                        data: data
                    });
                };

                // Track page load
                window.__servedTrackEvent('pageLoad', { url: window.location.href });

                // Track route changes
                const originalPushState = history.pushState;
                history.pushState = function() {
                    originalPushState.apply(this, arguments);
                    window.__servedTrackEvent('routeChange', { url: window.location.href });
                };

                // Track clicks
                document.addEventListener('click', function(e) {
                    const target = e.target;
                    window.__servedTrackEvent('click', {
                        tagName: target.tagName,
                        id: target.id,
                        className: target.className,
                        text: target.textContent?.substring(0, 100)
                    });
                });

                // Track errors
                window.addEventListener('error', function(e) {
                    window.__servedTrackEvent('error', {
                        message: e.message,
                        filename: e.filename,
                        lineno: e.lineno
                    });
                });
            ");
        }

        TrackEvent(BrowserEventType.SessionStart, new { sessionId = _sessionId });
    }

    /// <summary>
    /// Stops tracking browser events.
    /// </summary>
    public void StopTracking()
    {
        if (!_isTracking) return;

        _stopwatch.Stop();
        _isTracking = false;

        TrackEvent(BrowserEventType.SessionEnd, new
        {
            sessionId = _sessionId,
            duration = _stopwatch.ElapsedMilliseconds
        });
    }

    /// <summary>
    /// Tracks a page load event.
    /// </summary>
    public void TrackPageLoad(string url)
    {
        TrackEvent(BrowserEventType.PageLoad, new { url });
    }

    /// <summary>
    /// Tracks a click event.
    /// </summary>
    public void TrackClick(string selector)
    {
        TrackEvent(BrowserEventType.Click, new { selector });
    }

    /// <summary>
    /// Tracks a route change event.
    /// </summary>
    public void TrackRouteChange(string fromUrl, string toUrl)
    {
        TrackEvent(BrowserEventType.RouteChange, new { fromUrl, toUrl });
    }

    /// <summary>
    /// Tracks a form submission event.
    /// </summary>
    public void TrackFormSubmit(string formId)
    {
        TrackEvent(BrowserEventType.FormSubmit, new { formId });
    }

    /// <summary>
    /// Tracks an API call event.
    /// </summary>
    public void TrackApiCall(string method, string url, int? statusCode = null, long? duration = null)
    {
        TrackEvent(BrowserEventType.ApiCall, new { method, url, statusCode, duration });
    }

    /// <summary>
    /// Tracks an error event.
    /// </summary>
    public void TrackError(string message, string? stack = null)
    {
        TrackEvent(BrowserEventType.Error, new { message, stack });
    }

    /// <summary>
    /// Tracks a performance metric event.
    /// </summary>
    public void TrackPerformance(string metric, double value)
    {
        TrackEvent(BrowserEventType.Performance, new { metric, value });
    }

    /// <summary>
    /// Tracks a custom event.
    /// </summary>
    public void TrackEvent(BrowserEventType type, object data)
    {
        var evt = new BrowserEvent
        {
            Type = type,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SessionId = _sessionId ?? "unknown",
            ElapsedMs = _stopwatch.ElapsedMilliseconds,
            Data = data
        };

        _events.Enqueue(evt);
    }

    /// <summary>
    /// Collects events from the browser's JavaScript context.
    /// </summary>
    public async Task<List<BrowserEvent>> CollectBrowserEventsAsync()
    {
        if (_client.Page == null) return new List<BrowserEvent>();

        try
        {
            var jsEvents = await _client.Page.EvaluateAsync<List<Dictionary<string, object>>>(
                "() => window.__servedTestEvents || []");

            var events = new List<BrowserEvent>();
            foreach (var jsEvent in jsEvents)
            {
                var eventType = jsEvent.TryGetValue("type", out var typeObj)
                    ? ParseEventType(typeObj?.ToString())
                    : BrowserEventType.Custom;

                events.Add(new BrowserEvent
                {
                    Type = eventType,
                    Timestamp = jsEvent.TryGetValue("timestamp", out var ts) ? Convert.ToInt64(ts) : 0,
                    SessionId = _sessionId ?? "unknown",
                    Data = jsEvent.TryGetValue("data", out var data) ? data : null
                });
            }

            // Clear browser events
            await _client.Page.EvaluateAsync("() => window.__servedTestEvents = []");

            return events;
        }
        catch
        {
            return new List<BrowserEvent>();
        }
    }

    /// <summary>
    /// Gets event metrics summary.
    /// </summary>
    public EventMetrics GetMetrics()
    {
        var eventsList = _events.ToArray();

        return new EventMetrics
        {
            TotalEvents = eventsList.Length,
            ByType = eventsList.GroupBy(e => e.Type).ToDictionary(g => g.Key, g => g.Count()),
            PageLoadCount = eventsList.Count(e => e.Type == BrowserEventType.PageLoad),
            ClickCount = eventsList.Count(e => e.Type == BrowserEventType.Click),
            ErrorCount = eventsList.Count(e => e.Type == BrowserEventType.Error),
            TotalDurationMs = _stopwatch.ElapsedMilliseconds
        };
    }

    /// <summary>
    /// Exports events to JSON.
    /// </summary>
    public string ExportToJson()
    {
        return JsonSerializer.Serialize(new
        {
            sessionId = _sessionId,
            events = _events.ToArray(),
            metrics = GetMetrics()
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Saves events to a file.
    /// </summary>
    public async Task SaveToFileAsync(string path)
    {
        var json = ExportToJson();
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "./");
        await File.WriteAllTextAsync(path, json);
    }

    private static BrowserEventType ParseEventType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "pageload" => BrowserEventType.PageLoad,
            "routechange" => BrowserEventType.RouteChange,
            "click" => BrowserEventType.Click,
            "formsubmit" => BrowserEventType.FormSubmit,
            "apicall" => BrowserEventType.ApiCall,
            "error" => BrowserEventType.Error,
            "performance" => BrowserEventType.Performance,
            _ => BrowserEventType.Custom
        };
    }
}

/// <summary>
/// Types of browser events.
/// </summary>
public enum BrowserEventType
{
    SessionStart,
    SessionEnd,
    PageLoad,
    RouteChange,
    Click,
    FormSubmit,
    ApiCall,
    Error,
    Performance,
    Custom
}

/// <summary>
/// Represents a browser event.
/// </summary>
public class BrowserEvent
{
    public BrowserEventType Type { get; set; }
    public long Timestamp { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public long ElapsedMs { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Event metrics summary.
/// </summary>
public class EventMetrics
{
    public int TotalEvents { get; set; }
    public Dictionary<BrowserEventType, int> ByType { get; set; } = new();
    public int PageLoadCount { get; set; }
    public int ClickCount { get; set; }
    public int ErrorCount { get; set; }
    public long TotalDurationMs { get; set; }
}

