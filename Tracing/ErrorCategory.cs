namespace Served.SDK.Tracing;

/// <summary>
/// Categories for automatic error classification.
/// </summary>
public enum ErrorCategory
{
    // Client Errors
    /// <summary>401 - Authentication required or failed.</summary>
    Authentication,

    /// <summary>403 - Access denied.</summary>
    Authorization,

    /// <summary>404 - Resource not found.</summary>
    NotFound,

    /// <summary>400, 422 - Invalid request data.</summary>
    Validation,

    /// <summary>429 - Too many requests.</summary>
    RateLimited,

    // Server Errors
    /// <summary>500 - Internal server error.</summary>
    ServerError,

    /// <summary>503 - Service temporarily unavailable.</summary>
    ServiceUnavailable,

    /// <summary>504, client timeout - Request timed out.</summary>
    Timeout,

    // Network Errors
    /// <summary>Connection could not be established.</summary>
    ConnectionFailure,

    /// <summary>DNS resolution failed.</summary>
    DnsResolution,

    /// <summary>TLS/SSL handshake failed.</summary>
    TlsHandshake,

    // SDK Errors
    /// <summary>JSON serialization/deserialization error.</summary>
    Serialization,

    /// <summary>Configuration error.</summary>
    Configuration,

    /// <summary>Unknown or unclassified error.</summary>
    Unknown
}

/// <summary>
/// Helper methods for error categorization.
/// </summary>
public static class ErrorCategoryHelper
{
    /// <summary>
    /// Categorize an HTTP status code.
    /// </summary>
    public static ErrorCategory CategorizeHttpStatus(int statusCode)
    {
        return statusCode switch
        {
            400 => ErrorCategory.Validation,
            401 => ErrorCategory.Authentication,
            403 => ErrorCategory.Authorization,
            404 => ErrorCategory.NotFound,
            422 => ErrorCategory.Validation,
            429 => ErrorCategory.RateLimited,
            500 => ErrorCategory.ServerError,
            502 => ErrorCategory.ServiceUnavailable,
            503 => ErrorCategory.ServiceUnavailable,
            504 => ErrorCategory.Timeout,
            >= 400 and < 500 => ErrorCategory.Validation,
            >= 500 => ErrorCategory.ServerError,
            _ => ErrorCategory.Unknown
        };
    }

    /// <summary>
    /// Categorize an exception.
    /// </summary>
    public static ErrorCategory CategorizeException(Exception exception)
    {
        return exception switch
        {
            TaskCanceledException or OperationCanceledException => ErrorCategory.Timeout,
            HttpRequestException httpEx when httpEx.Message.Contains("SSL") ||
                                             httpEx.Message.Contains("TLS") => ErrorCategory.TlsHandshake,
            HttpRequestException httpEx when httpEx.Message.Contains("DNS") ||
                                             httpEx.Message.Contains("host") => ErrorCategory.DnsResolution,
            HttpRequestException => ErrorCategory.ConnectionFailure,
            System.Text.Json.JsonException or Newtonsoft.Json.JsonException => ErrorCategory.Serialization,
            ArgumentException or InvalidOperationException => ErrorCategory.Configuration,
            _ => ErrorCategory.Unknown
        };
    }

    /// <summary>
    /// Get the default severity for an error category.
    /// </summary>
    public static TelemetrySeverity GetDefaultSeverity(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.NotFound => TelemetrySeverity.Warning,
            ErrorCategory.Validation => TelemetrySeverity.Warning,
            ErrorCategory.RateLimited => TelemetrySeverity.Warning,
            ErrorCategory.Authentication => TelemetrySeverity.Error,
            ErrorCategory.Authorization => TelemetrySeverity.Error,
            ErrorCategory.ServerError => TelemetrySeverity.Error,
            ErrorCategory.ServiceUnavailable => TelemetrySeverity.Error,
            ErrorCategory.Timeout => TelemetrySeverity.Error,
            ErrorCategory.ConnectionFailure => TelemetrySeverity.Error,
            ErrorCategory.DnsResolution => TelemetrySeverity.Critical,
            ErrorCategory.TlsHandshake => TelemetrySeverity.Critical,
            ErrorCategory.Configuration => TelemetrySeverity.Critical,
            _ => TelemetrySeverity.Error
        };
    }
}
