using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Served.SDK.Utilities;

/// <summary>
/// Extension methods for parsing tool arguments from Dictionary&lt;string, object&gt;.
/// Used by backend MCP tool handlers for consistent parameter extraction.
/// </summary>
public static class DictToolArgs
{
    #region Required Parameters

    /// <summary>
    /// Get required integer parameter. Throws if missing or invalid.
    /// </summary>
    public static int GetRequiredInt(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            throw new ArgumentException($"{name} required");

        return ConvertToInt(value) ?? throw new ArgumentException($"{name} must be a valid integer");
    }

    /// <summary>
    /// Get required long parameter. Throws if missing or invalid.
    /// </summary>
    public static long GetRequiredLong(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            throw new ArgumentException($"{name} required");

        return ConvertToLong(value) ?? throw new ArgumentException($"{name} must be a valid long");
    }

    /// <summary>
    /// Get required string parameter. Throws if missing or empty.
    /// </summary>
    public static string GetRequiredString(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            throw new ArgumentException($"{name} required");

        var strValue = ConvertToString(value);
        if (string.IsNullOrWhiteSpace(strValue))
            throw new ArgumentException($"{name} required");

        return strValue;
    }

    /// <summary>
    /// Get required boolean parameter. Throws if missing.
    /// </summary>
    public static bool GetRequiredBool(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            throw new ArgumentException($"{name} required");

        return ConvertToBool(value) ?? throw new ArgumentException($"{name} must be a valid boolean");
    }

    /// <summary>
    /// Get required DateTime parameter. Throws if missing or invalid.
    /// </summary>
    public static DateTime GetRequiredDateTime(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            throw new ArgumentException($"{name} required");

        return ConvertToDateTime(value) ?? throw new ArgumentException($"{name} must be a valid date/time");
    }

    /// <summary>
    /// Get required enum parameter. Throws if missing or invalid.
    /// </summary>
    public static T GetRequiredEnum<T>(this Dictionary<string, object> args, string name) where T : struct, Enum
    {
        var value = ConvertToString(args.GetValueOrDefault(name));
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
    public static int GetOptionalInt(this Dictionary<string, object> args, string name, int defaultValue = 0)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        return ConvertToInt(value) ?? defaultValue;
    }

    /// <summary>
    /// Get optional nullable integer parameter.
    /// </summary>
    public static int? GetOptionalIntOrNull(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return null;

        return ConvertToInt(value);
    }

    /// <summary>
    /// Get optional long parameter with default value.
    /// </summary>
    public static long GetOptionalLong(this Dictionary<string, object> args, string name, long defaultValue = 0)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        return ConvertToLong(value) ?? defaultValue;
    }

    /// <summary>
    /// Get optional nullable long parameter.
    /// </summary>
    public static long? GetOptionalLongOrNull(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return null;

        return ConvertToLong(value);
    }

    /// <summary>
    /// Get optional string parameter with default value.
    /// </summary>
    public static string? GetOptionalString(this Dictionary<string, object> args, string name, string? defaultValue = null)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        var strValue = ConvertToString(value);
        return string.IsNullOrWhiteSpace(strValue) ? defaultValue : strValue;
    }

    /// <summary>
    /// Get optional boolean parameter with default value.
    /// </summary>
    public static bool GetOptionalBool(this Dictionary<string, object> args, string name, bool defaultValue = false)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        return ConvertToBool(value) ?? defaultValue;
    }

    /// <summary>
    /// Get optional DateTime parameter.
    /// </summary>
    public static DateTime? GetOptionalDateTime(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return null;

        return ConvertToDateTime(value);
    }

    /// <summary>
    /// Get optional enum parameter with default value.
    /// </summary>
    public static T GetOptionalEnum<T>(this Dictionary<string, object> args, string name, T defaultValue) where T : struct, Enum
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        var strValue = ConvertToString(value);
        if (string.IsNullOrWhiteSpace(strValue))
            return defaultValue;

        return Enum.TryParse<T>(strValue, true, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Get optional nullable enum parameter.
    /// </summary>
    public static T? GetOptionalEnumOrNull<T>(this Dictionary<string, object> args, string name) where T : struct, Enum
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return null;

        var strValue = ConvertToString(value);
        if (string.IsNullOrWhiteSpace(strValue))
            return null;

        return Enum.TryParse<T>(strValue, true, out var result) ? result : null;
    }

    /// <summary>
    /// Get optional double parameter with default value.
    /// </summary>
    public static double GetOptionalDouble(this Dictionary<string, object> args, string name, double defaultValue = 0)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return defaultValue;

        return ConvertToDouble(value) ?? defaultValue;
    }

    /// <summary>
    /// Get optional nullable double parameter.
    /// </summary>
    public static double? GetOptionalDoubleOrNull(this Dictionary<string, object> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value == null)
            return null;

        return ConvertToDouble(value);
    }

    #endregion

    #region Collection Parameters

    /// <summary>
    /// Get list of strings from array or comma-separated string.
    /// </summary>
    public static List<string> GetStringList(this Dictionary<string, object> args, string name, bool required = false)
    {
        var result = new List<string>();

        if (!args.TryGetValue(name, out var value) || value == null)
        {
            if (required)
                throw new ArgumentException($"{name} required");
            return result;
        }

        if (value is IEnumerable<string> stringList)
        {
            result.AddRange(stringList.Where(s => !string.IsNullOrWhiteSpace(s)));
        }
        else if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in jsonElement.EnumerateArray())
                {
                    var str = item.GetString();
                    if (!string.IsNullOrWhiteSpace(str))
                        result.Add(str);
                }
            }
            else if (jsonElement.ValueKind == JsonValueKind.String)
            {
                var str = jsonElement.GetString();
                if (!string.IsNullOrWhiteSpace(str))
                    result.AddRange(str.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
        }
        else if (value is IEnumerable<object> objectList)
        {
            foreach (var item in objectList)
            {
                var str = ConvertToString(item);
                if (!string.IsNullOrWhiteSpace(str))
                    result.Add(str);
            }
        }
        else
        {
            var str = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(str))
                result.AddRange(str.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required");

        return result;
    }

    /// <summary>
    /// Get list of integers from array.
    /// </summary>
    public static List<int> GetIntList(this Dictionary<string, object> args, string name, bool required = false)
    {
        var result = new List<int>();

        if (!args.TryGetValue(name, out var value) || value == null)
        {
            if (required)
                throw new ArgumentException($"{name} required");
            return result;
        }

        if (value is IEnumerable<int> intList)
        {
            result.AddRange(intList);
        }
        else if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in jsonElement.EnumerateArray())
            {
                if (item.TryGetInt32(out var num))
                    result.Add(num);
            }
        }
        else if (value is IEnumerable<object> objectList)
        {
            foreach (var item in objectList)
            {
                var num = ConvertToInt(item);
                if (num.HasValue)
                    result.Add(num.Value);
            }
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required");

        return result;
    }

    /// <summary>
    /// Get list of longs from array.
    /// </summary>
    public static List<long> GetLongList(this Dictionary<string, object> args, string name, bool required = false)
    {
        var result = new List<long>();

        if (!args.TryGetValue(name, out var value) || value == null)
        {
            if (required)
                throw new ArgumentException($"{name} required");
            return result;
        }

        if (value is IEnumerable<long> longList)
        {
            result.AddRange(longList);
        }
        else if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in jsonElement.EnumerateArray())
            {
                if (item.TryGetInt64(out var num))
                    result.Add(num);
            }
        }
        else if (value is IEnumerable<object> objectList)
        {
            foreach (var item in objectList)
            {
                var num = ConvertToLong(item);
                if (num.HasValue)
                    result.Add(num.Value);
            }
        }

        if (required && result.Count == 0)
            throw new ArgumentException($"{name} required");

        return result;
    }

    #endregion

    #region Validation Helpers

    /// <summary>
    /// Validate that at least one of the specified parameters is provided.
    /// </summary>
    public static void RequireAny(this Dictionary<string, object> args, params string[] names)
    {
        foreach (var name in names)
        {
            if (args.TryGetValue(name, out var value) && value != null)
                return;
        }

        throw new ArgumentException($"At least one of [{string.Join(", ", names)}] is required");
    }

    /// <summary>
    /// Check if a parameter exists and has a value.
    /// </summary>
    public static bool Has(this Dictionary<string, object> args, string name)
    {
        return args.TryGetValue(name, out var value) && value != null;
    }

    #endregion

    #region Conversion Helpers

    private static int? ConvertToInt(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            int i => i,
            long l => (int)l,
            double d => (int)d,
            float f => (int)f,
            decimal dec => (int)dec,
            string s when int.TryParse(s, out var parsed) => parsed,
            JsonElement je when je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var num) => num,
            JsonElement je when je.ValueKind == JsonValueKind.String && int.TryParse(je.GetString(), out var parsed) => parsed,
            _ => null
        };
    }

    private static long? ConvertToLong(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            long l => l,
            int i => i,
            double d => (long)d,
            float f => (long)f,
            decimal dec => (long)dec,
            string s when long.TryParse(s, out var parsed) => parsed,
            JsonElement je when je.ValueKind == JsonValueKind.Number && je.TryGetInt64(out var num) => num,
            JsonElement je when je.ValueKind == JsonValueKind.String && long.TryParse(je.GetString(), out var parsed) => parsed,
            _ => null
        };
    }

    private static double? ConvertToDouble(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            double d => d,
            float f => f,
            int i => i,
            long l => l,
            decimal dec => (double)dec,
            string s when double.TryParse(s, out var parsed) => parsed,
            JsonElement je when je.ValueKind == JsonValueKind.Number && je.TryGetDouble(out var num) => num,
            JsonElement je when je.ValueKind == JsonValueKind.String && double.TryParse(je.GetString(), out var parsed) => parsed,
            _ => null
        };
    }

    private static string? ConvertToString(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            string s => s,
            JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
            JsonElement je => je.ToString(),
            _ => value.ToString()
        };
    }

    private static bool? ConvertToBool(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            bool b => b,
            string s when bool.TryParse(s, out var parsed) => parsed,
            string s when s.Equals("1", StringComparison.OrdinalIgnoreCase) => true,
            string s when s.Equals("0", StringComparison.OrdinalIgnoreCase) => false,
            int i => i != 0,
            long l => l != 0,
            JsonElement je when je.ValueKind == JsonValueKind.True => true,
            JsonElement je when je.ValueKind == JsonValueKind.False => false,
            JsonElement je when je.ValueKind == JsonValueKind.String && bool.TryParse(je.GetString(), out var parsed) => parsed,
            _ => null
        };
    }

    private static DateTime? ConvertToDateTime(object? value)
    {
        if (value == null) return null;

        return value switch
        {
            DateTime dt => dt,
            DateTimeOffset dto => dto.DateTime,
            string s when DateTime.TryParse(s, out var parsed) => parsed,
            JsonElement je when je.ValueKind == JsonValueKind.String && DateTime.TryParse(je.GetString(), out var parsed) => parsed,
            _ => null
        };
    }

    #endregion
}
