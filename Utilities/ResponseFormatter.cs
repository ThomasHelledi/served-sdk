using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Served.SDK.Utilities;

/// <summary>
/// Fluent response formatter for tool outputs.
/// Generates consistent @entity[id] { ... } formatted responses.
/// </summary>
public class ResponseFormatter
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel = 0;
    private const string IndentUnit = "  ";

    /// <summary>
    /// Create a new ResponseFormatter.
    /// </summary>
    public static ResponseFormatter Create() => new();

    #region Headers

    /// <summary>
    /// Add a header line (e.g., "Found 5 projects:").
    /// </summary>
    public ResponseFormatter Header(string text)
    {
        _sb.AppendLine(text);
        _sb.AppendLine();
        return this;
    }

    /// <summary>
    /// Add a count header (e.g., "Found 5 projects:").
    /// </summary>
    public ResponseFormatter CountHeader(int count, string entityName, string? suffix = null)
    {
        var plural = count == 1 ? entityName : $"{entityName}s";
        var text = suffix != null
            ? $"Found {count} {plural} {suffix}:"
            : $"Found {count} {plural}:";
        return Header(text);
    }

    /// <summary>
    /// Add a Danish count header (e.g., "Her er 5 projekter:").
    /// </summary>
    public ResponseFormatter CountHeaderDa(int count, string singularName, string pluralName)
    {
        var name = count == 1 ? singularName : pluralName;
        _sb.AppendLine($"Her er {count} {name}:");
        _sb.AppendLine();
        return this;
    }

    #endregion

    #region Entity Formatting

    /// <summary>
    /// Start an entity block. Use with EndEntity().
    /// </summary>
    public ResponseFormatter Entity(string type, object id)
    {
        Indent();
        _sb.AppendLine($"@{type}[{id}] {{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// End an entity block.
    /// </summary>
    public ResponseFormatter EndEntity()
    {
        _indentLevel--;
        Indent();
        _sb.AppendLine("}");
        _sb.AppendLine();
        return this;
    }

    /// <summary>
    /// Add a property to current entity.
    /// </summary>
    public ResponseFormatter Prop(string name, object? value)
    {
        Indent();
        _sb.AppendLine($"{name}: {FormatValue(value)}");
        return this;
    }

    /// <summary>
    /// Add a property only if value is not null/empty.
    /// </summary>
    public ResponseFormatter PropIfNotEmpty(string name, object? value)
    {
        if (value == null) return this;
        if (value is string s && string.IsNullOrWhiteSpace(s)) return this;

        return Prop(name, value);
    }

    /// <summary>
    /// Add a string property with quotes.
    /// </summary>
    public ResponseFormatter PropString(string name, string? value)
    {
        Indent();
        _sb.AppendLine($"{name}: \"{value ?? ""}\"");
        return this;
    }

    /// <summary>
    /// Add a string property only if not null/empty.
    /// </summary>
    public ResponseFormatter PropStringIfNotEmpty(string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return this;
        return PropString(name, value);
    }

    /// <summary>
    /// Add a date property formatted as yyyy-MM-dd.
    /// </summary>
    public ResponseFormatter PropDate(string name, DateTime? value)
    {
        if (!value.HasValue) return this;
        Indent();
        _sb.AppendLine($"{name}: \"{value.Value:yyyy-MM-dd}\"");
        return this;
    }

    /// <summary>
    /// Add a datetime property formatted as yyyy-MM-dd HH:mm.
    /// </summary>
    public ResponseFormatter PropDateTime(string name, DateTime? value, string? fallback = null)
    {
        Indent();
        var formatted = value?.ToString("yyyy-MM-dd HH:mm") ?? fallback ?? "null";
        _sb.AppendLine($"{name}: \"{formatted}\"");
        return this;
    }

    /// <summary>
    /// Add a boolean property as lowercase.
    /// </summary>
    public ResponseFormatter PropBool(string name, bool value)
    {
        Indent();
        _sb.AppendLine($"{name}: {value.ToString().ToLowerInvariant()}");
        return this;
    }

    /// <summary>
    /// Add a list/array property.
    /// </summary>
    public ResponseFormatter PropList(string name, IEnumerable<string>? values)
    {
        var list = values?.ToList() ?? new List<string>();
        Indent();
        _sb.AppendLine($"{name}: [{string.Join(", ", list.Select(v => $"\"{v}\""))}]");
        return this;
    }

    /// <summary>
    /// Add a count property with nested collection.
    /// </summary>
    public ResponseFormatter PropCount(string name, int count)
    {
        Indent();
        _sb.AppendLine($"{name}: [{count}] {{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// End a count property block.
    /// </summary>
    public ResponseFormatter EndPropCount()
    {
        _indentLevel--;
        Indent();
        _sb.AppendLine("}}");
        return this;
    }

    /// <summary>
    /// Add a nested entity within a property.
    /// </summary>
    public ResponseFormatter NestedEntity(string type, object id)
    {
        Indent();
        _sb.AppendLine($"@{type}[{id}] {{");
        _indentLevel++;
        return this;
    }

    /// <summary>
    /// End a nested entity.
    /// </summary>
    public ResponseFormatter EndNestedEntity()
    {
        _indentLevel--;
        Indent();
        _sb.AppendLine("}");
        return this;
    }

    #endregion

    #region Collection Formatting

    /// <summary>
    /// Format a list of entities with a callback for each.
    /// </summary>
    public ResponseFormatter ForEach<T>(IEnumerable<T> items, Action<ResponseFormatter, T> formatter)
    {
        foreach (var item in items)
        {
            formatter(this, item);
        }
        return this;
    }

    /// <summary>
    /// Format a list of entities.
    /// </summary>
    public ResponseFormatter EntityList<T>(
        string entityType,
        IEnumerable<T> items,
        Func<T, object> getId,
        Action<ResponseFormatter, T> formatProps)
    {
        foreach (var item in items)
        {
            Entity(entityType, getId(item));
            formatProps(this, item);
            EndEntity();
        }
        return this;
    }

    #endregion

    #region Text Formatting

    /// <summary>
    /// Add a raw line.
    /// </summary>
    public ResponseFormatter Line(string text = "")
    {
        if (string.IsNullOrEmpty(text))
            _sb.AppendLine();
        else
        {
            Indent();
            _sb.AppendLine(text);
        }
        return this;
    }

    /// <summary>
    /// Add a bullet point line.
    /// </summary>
    public ResponseFormatter Bullet(string text)
    {
        Indent();
        _sb.AppendLine($"- {text}");
        return this;
    }

    /// <summary>
    /// Add a success message.
    /// </summary>
    public ResponseFormatter Success(string message)
    {
        _sb.AppendLine(message);
        return this;
    }

    /// <summary>
    /// Add an info message.
    /// </summary>
    public ResponseFormatter Info(string message)
    {
        _sb.AppendLine();
        _sb.AppendLine(message);
        return this;
    }

    /// <summary>
    /// Add a warning message.
    /// </summary>
    public ResponseFormatter Warning(string message)
    {
        _sb.AppendLine();
        _sb.AppendLine($"⚠️ {message}");
        return this;
    }

    #endregion

    #region Result Messages

    /// <summary>
    /// Create a simple entity created message.
    /// </summary>
    public static string Created(string entityType, object id, string name)
        => $"{entityType} created: @{entityType.ToLowerInvariant()}[{id}] \"{name}\"";

    /// <summary>
    /// Create a simple entity updated message.
    /// </summary>
    public static string Updated(string entityType, object id)
        => $"{entityType} {id} updated.";

    /// <summary>
    /// Create a simple entity deleted message.
    /// </summary>
    public static string Deleted(string entityType, object id)
        => $"{entityType} {id} deleted.";

    /// <summary>
    /// Create a Danish created message.
    /// </summary>
    public static string CreatedDa(string entityType, object id, string name)
        => $"{entityType} oprettet: @{entityType.ToLowerInvariant()}[{id}] \"{name}\"";

    /// <summary>
    /// Create a Danish updated message.
    /// </summary>
    public static string UpdatedDa(string entityType, object id)
        => $"{entityType} {id} opdateret.";

    /// <summary>
    /// Create a Danish deleted message.
    /// </summary>
    public static string DeletedDa(string entityType, object id)
        => $"{entityType} {id} slettet.";

    #endregion

    #region Output

    /// <summary>
    /// Build the final string output.
    /// </summary>
    public override string ToString() => _sb.ToString();

    /// <summary>
    /// Implicit conversion to string.
    /// </summary>
    public static implicit operator string(ResponseFormatter formatter) => formatter.ToString();

    #endregion

    #region Private Helpers

    private void Indent()
    {
        for (int i = 0; i < _indentLevel; i++)
            _sb.Append(IndentUnit);
    }

    private static string FormatValue(object? value) => value switch
    {
        null => "null",
        bool b => b.ToString().ToLowerInvariant(),
        string s => $"\"{s}\"",
        DateTime dt => $"\"{dt:yyyy-MM-dd HH:mm}\"",
        Enum e => $"\"{e}\"",
        _ => value.ToString() ?? "null"
    };

    #endregion
}
