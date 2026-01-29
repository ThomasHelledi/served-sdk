using System;

namespace Served.SDK.Exceptions;

/// <summary>
/// Represents errors that occur during Served API execution.
/// </summary>
public class ServedApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the raw response content from the API.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServedApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseContent">The response content.</param>
    public ServedApiException(string message, int statusCode, string? responseContent) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}
