using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Served.SDK.Utilities;

/// <summary>
/// Extension methods for parsing tool arguments from JObject.
/// Provides consistent parameter extraction with proper error handling.
/// </summary>
public static class ToolArgs
{
    #region Required Parameters

    /// <summary>
    /// Get required integer parameter. Throws if missing or invalid.
    /// </summary>
    public static int GetRequiredInt(this JObject args, string name)
    {
        var token = args[name];
        if (token == null)
            throw new ArgumentException($"{name} required");

        return token.Value<int?>() ?? throw new ArgumentException($"{name} must be a valid integer");
    }

    /// <summary>
    /// Get required long parameter. Throws if missing or invalid.
    /// </summary>
    public static long GetRequiredLong(this JObject args, string name)
    {
        var token = args[name];
        if (token == null)
            throw new ArgumentException($"{name} required");

        return token.Value<long?>() ?? throw new ArgumentException($"{name} must be a valid long");
    }

    /// <summary>
    /// Get required string parameter. Throws if missing or empty.
    /// </summary>
    public static string GetRequiredString(this JObject args, string name)
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} required");

        return value;
    }

    /// <summary>
    /// Get required boolean parameter. Throws if missing.
    /// </summary>
    public static bool GetRequiredBool(this JObject args, string name)
    {
        var token = args[name];
        if (token == null)
            throw new ArgumentException($"{name} required");

        return token.Value<bool?>() ?? throw new ArgumentException($"{name} must be a valid boolean");
    }

    /// <summary>
    /// Get required DateTime parameter. Throws if missing or invalid.
    /// </summary>
    public static DateTime GetRequiredDateTime(this JObject args, string name)
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} required");

        if (!DateTime.TryParse(value, out var result))
            throw new ArgumentException($"{name} must be a valid date/time");

        return result;
    }

    /// <summary>
    /// Get required enum parameter. Throws if missing or invalid.
    /// </summary>
    public static T GetRequiredEnum<T>(this JObject args, string name) where T : struct, Enum
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} required");

        if (!Enum.TryParse<T>(value, true, out var result))
            throw new ArgumentException($"{name} must be one of: {string.Join(", ", Enum.GetNames<T>())}");

        return result;
    }

    #endregion

    #region Optional Parameters

    /// <summary>
    /// Get optional integer parameter with default value.
    /// </summary>
    public static int GetOptionalInt(this JObject args, string name, int defaultValue = 0)
    {
        return args[name]?.Value<int?>() ?? defaultValue;
    }

    /// <summary>
    /// Get optional nullable integer parameter.
    /// </summary>
    public static int? GetOptionalIntOrNull(this JObject args, string name)
    {
        return args[name]?.Value<int?>();
    }

    /// <summary>
    /// Get optional long parameter with default value.
    /// </summary>
    public static long GetOptionalLong(this JObject args, string name, long defaultValue = 0)
    {
        return args[name]?.Value<long?>() ?? defaultValue;
    }

    /// <summary>
    /// Get optional string parameter with default value.
    /// </summary>
    public static string? GetOptionalString(this JObject args, string name, string? defaultValue = null)
    {
        var value = args[name]?.Value<string>();
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    /// <summary>
    /// Get optional boolean parameter with default value.
    /// </summary>
    public static bool GetOptionalBool(this JObject args, string name, bool defaultValue = false)
    {
        return args[name]?.Value<bool?>() ?? defaultValue;
    }

    /// <summary>
    /// Get optional DateTime parameter.
    /// </summary>
    public static DateTime? GetOptionalDateTime(this JObject args, string name)
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return DateTime.TryParse(value, out var result) ? result : null;
    }

    /// <summary>
    /// Get optional enum parameter with default value.
    /// </summary>
    public static T GetOptionalEnum<T>(this JObject args, string name, T defaultValue) where T : struct, Enum
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Get optional nullable enum parameter.
    /// </summary>
    public static T? GetOptionalEnumOrNull<T>(this JObject args, string name) where T : struct, Enum
    {
        var value = args[name]?.Value<string>();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Enum.TryParse<T>(value, true, out var result) ? result : null;
    }

    #endregion

    #region Collection Parameters

    /// <summary>
    /// Get list of strings from array or comma-separated string.
    /// </summary>
    public static List<string> GetStringList(this JObject args, string name, bool required = false)
    {
        var result = new List<string>();
        var token = args[name];

        if (token is JArray array)
        {
            foreach (var item in array)
            {
                var value = item.Value<string>();
                if (!string.IsNullOrWhiteSpace(value))
                    result.Add(value);
            }
        }
        else if (token != null)
        {
            var value = token.Value<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                result.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required (array or comma-separated string)");

        return result;
    }

    /// <summary>
    /// Get list of integers from array.
    /// </summary>
    public static List<int> GetIntList(this JObject args, string name, bool required = false)
    {
        var result = new List<int>();
        var token = args[name];

        if (token is JArray array)
        {
            foreach (var item in array)
            {
                var value = item.Value<int?>();
                if (value.HasValue)
                    result.Add(value.Value);
            }
        }
        else if (token != null)
        {
            var value = token.Value<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (int.TryParse(part, out var num))
                        result.Add(num);
                }
            }
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required");

        return result;
    }

    /// <summary>
    /// Get JArray from arguments. Throws if required and missing.
    /// </summary>
    public static JArray? GetArray(this JObject args, string name, bool required = false)
    {
        var token = args[name];

        if (token is JArray array)
            return array;

        if (required)
            throw new ArgumentException($"{name} required (array)");

        return null;
    }

    /// <summary>
    /// Get typed list from JArray.
    /// </summary>
    public static List<T> GetTypedList<T>(this JObject args, string name, bool required = false) where T : class
    {
        var result = new List<T>();
        var token = args[name];

        if (token is JArray array)
        {
            foreach (var item in array)
            {
                var obj = item.ToObject<T>();
                if (obj != null)
                    result.Add(obj);
            }
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required");

        return result;
    }

    #endregion

    #region Object Parameters

    /// <summary>
    /// Get nested object and convert to type.
    /// </summary>
    public static T? GetObject<T>(this JObject args, string name, bool required = false) where T : class
    {
        var token = args[name];
        if (token == null || token.Type == JTokenType.Null)
        {
            if (required)
                throw new ArgumentException($"{name} required");
            return null;
        }

        return token.ToObject<T>();
    }

    /// <summary>
    /// Get nested JObject.
    /// </summary>
    public static JObject? GetJObject(this JObject args, string name, bool required = false)
    {
        var token = args[name];
        if (token is JObject obj)
            return obj;

        if (required)
            throw new ArgumentException($"{name} required (object)");

        return null;
    }

    #endregion

    #region Validation Helpers

    /// <summary>
    /// Validate that at least one of the specified parameters is provided.
    /// </summary>
    public static void RequireAny(this JObject args, params string[] names)
    {
        foreach (var name in names)
        {
            var token = args[name];
            if (token != null && token.Type != JTokenType.Null)
                return;
        }

        throw new ArgumentException($"At least one of [{string.Join(", ", names)}] is required");
    }

    /// <summary>
    /// Check if a parameter exists and has a value.
    /// </summary>
    public static bool Has(this JObject args, string name)
    {
        var token = args[name];
        return token != null && token.Type != JTokenType.Null;
    }

    #endregion
}
