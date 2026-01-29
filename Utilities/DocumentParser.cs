using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Served.SDK.Utilities;

/// <summary>
/// Parses unified format documentation files (.unified.md, .unified.json, .unified.yaml)
/// and provides tool metadata and documentation lookup.
/// </summary>
public class DocumentParser
{
    private readonly string _docsBasePath;
    private readonly Dictionary<string, ToolDocumentation> _toolDocs = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _toolToFile = new(StringComparer.OrdinalIgnoreCase);
    private bool _loaded = false;

    /// <summary>
    /// Create a DocumentParser with optional custom docs path.
    /// </summary>
    public DocumentParser(string? docsBasePath = null)
    {
        _docsBasePath = docsBasePath ?? GetDefaultDocsPath();
    }

    #region Loading

    /// <summary>
    /// Load all documentation files from the docs directory.
    /// </summary>
    public void Load()
    {
        if (_loaded) return;

        if (!Directory.Exists(_docsBasePath))
        {
            Console.Error.WriteLine($"[DocumentParser] Docs path not found: {_docsBasePath}");
            _loaded = true;
            return;
        }

        var files = Directory.GetFiles(_docsBasePath, "*.unified.md", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(_docsBasePath, "*.unified.json", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(_docsBasePath, "*.unified.yaml", SearchOption.AllDirectories));

        foreach (var file in files)
        {
            try
            {
                ParseFile(file);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[DocumentParser] Error parsing {file}: {ex.Message}");
            }
        }

        Console.Error.WriteLine($"[DocumentParser] Loaded {_toolDocs.Count} tool definitions from {files.Count()} files");
        _loaded = true;
    }

    /// <summary>
    /// Ensure documentation is loaded.
    /// </summary>
    public void EnsureLoaded()
    {
        if (!_loaded) Load();
    }

    #endregion

    #region Lookup

    /// <summary>
    /// Get documentation for a specific tool.
    /// </summary>
    public ToolDocumentation? GetToolDoc(string toolName)
    {
        EnsureLoaded();
        return _toolDocs.TryGetValue(toolName, out var doc) ? doc : null;
    }

    /// <summary>
    /// Check if a tool has documentation.
    /// </summary>
    public bool HasDocumentation(string toolName)
    {
        EnsureLoaded();
        return _toolDocs.ContainsKey(toolName);
    }

    /// <summary>
    /// Get all documented tools.
    /// </summary>
    public IReadOnlyCollection<string> GetAllToolNames()
    {
        EnsureLoaded();
        return _toolDocs.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// Get all tool documentation objects.
    /// </summary>
    public IReadOnlyCollection<ToolDocumentation> GetAllToolDocs()
    {
        EnsureLoaded();
        return _toolDocs.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Get tools by domain.
    /// </summary>
    public IEnumerable<ToolDocumentation> GetToolsByDomain(string domain)
    {
        EnsureLoaded();
        return _toolDocs.Values.Where(t => t.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get tools by tag.
    /// </summary>
    public IEnumerable<ToolDocumentation> GetToolsByTag(string tag)
    {
        EnsureLoaded();
        return _toolDocs.Values.Where(t => t.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Search tools by name or description.
    /// </summary>
    public IEnumerable<ToolDocumentation> SearchTools(string query)
    {
        EnsureLoaded();
        var q = query.ToLowerInvariant();
        return _toolDocs.Values.Where(t =>
            t.Name.ToLowerInvariant().Contains(q) ||
            t.Description.ToLowerInvariant().Contains(q) ||
            t.Parameters.Any(p => p.Name.ToLowerInvariant().Contains(q)));
    }

    #endregion

    #region Validation

    /// <summary>
    /// Get undocumented tools.
    /// </summary>
    public List<string> GetUndocumentedTools(IEnumerable<string> registeredTools)
    {
        EnsureLoaded();
        return registeredTools.Where(t => !_toolDocs.ContainsKey(t)).ToList();
    }

    /// <summary>
    /// Get orphaned documentation (documented but not registered).
    /// </summary>
    public List<string> GetOrphanedDocs(IEnumerable<string> registeredTools)
    {
        EnsureLoaded();
        var registered = new HashSet<string>(registeredTools, StringComparer.OrdinalIgnoreCase);
        return _toolDocs.Keys.Where(t => !registered.Contains(t)).ToList();
    }

    /// <summary>
    /// Validate documentation coverage.
    /// </summary>
    public DocumentationValidationResult Validate(IEnumerable<string> registeredTools)
    {
        EnsureLoaded();
        return new DocumentationValidationResult
        {
            UndocumentedTools = GetUndocumentedTools(registeredTools),
            OrphanedDocs = GetOrphanedDocs(registeredTools),
            TotalTools = registeredTools.Count(),
            DocumentedTools = registeredTools.Count(t => _toolDocs.ContainsKey(t))
        };
    }

    #endregion

    #region Parsing

    private void ParseFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        if (extension == ".md")
        {
            ParseMarkdownFile(filePath, content);
        }
    }

    private void ParseMarkdownFile(string filePath, string content)
    {
        var frontmatterMatch = Regex.Match(content, @"^---\s*\n(.*?)\n---", RegexOptions.Singleline);
        if (!frontmatterMatch.Success) return;

        var frontmatter = frontmatterMatch.Groups[1].Value;
        var bodyContent = content.Substring(frontmatterMatch.Length).Trim();

        var metadata = ParseYamlFrontmatter(frontmatter);
        if (metadata.Type != "mcp-tool") return;

        var tools = ParseToolsFromMarkdown(bodyContent, metadata);
        foreach (var tool in tools)
        {
            tool.SourceFile = filePath;
            _toolDocs[tool.Name] = tool;
            _toolToFile[tool.Name] = filePath;
        }
    }

    private DocumentMetadata ParseYamlFrontmatter(string yaml)
    {
        var metadata = new DocumentMetadata();

        foreach (var line in yaml.Split('\n'))
        {
            var colonIndex = line.IndexOf(':');
            if (colonIndex <= 0) continue;

            var key = line.Substring(0, colonIndex).Trim();
            var value = line.Substring(colonIndex + 1).Trim();

            switch (key.ToLowerInvariant())
            {
                case "type": metadata.Type = value; break;
                case "name": metadata.Name = value; break;
                case "version": metadata.Version = value; break;
                case "domain": metadata.Domain = value; break;
                case "tags": metadata.Tags = ParseYamlArray(value); break;
                case "description": metadata.Description = value; break;
            }
        }

        return metadata;
    }

    private List<string> ParseYamlArray(string value)
    {
        value = value.Trim();
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
            return value.Split(',')
                .Select(s => s.Trim().Trim('"', '\''))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }
        return new List<string>();
    }

    private List<ToolDocumentation> ParseToolsFromMarkdown(string content, DocumentMetadata metadata)
    {
        var tools = new List<ToolDocumentation>();
        var toolSections = Regex.Matches(content, @"^##\s+(\w+)\s*$", RegexOptions.Multiline);

        for (int i = 0; i < toolSections.Count; i++)
        {
            var toolName = toolSections[i].Groups[1].Value;
            var startIndex = toolSections[i].Index + toolSections[i].Length;
            var endIndex = i + 1 < toolSections.Count ? toolSections[i + 1].Index : content.Length;
            var toolContent = content.Substring(startIndex, endIndex - startIndex).Trim();

            var tool = ParseToolSection(toolName, toolContent, metadata);
            tools.Add(tool);
        }

        if (tools.Count == 0 && !string.IsNullOrWhiteSpace(metadata.Name))
        {
            tools.Add(new ToolDocumentation
            {
                Name = metadata.Name,
                Description = metadata.Description,
                Domain = metadata.Domain,
                Version = metadata.Version,
                Tags = metadata.Tags
            });
        }

        return tools;
    }

    private ToolDocumentation ParseToolSection(string name, string content, DocumentMetadata metadata)
    {
        var tool = new ToolDocumentation
        {
            Name = name,
            Domain = metadata.Domain,
            Version = metadata.Version,
            Tags = metadata.Tags
        };

        var descMatch = Regex.Match(content, @"^(.+?)(?:\n\n|\n###)", RegexOptions.Singleline);
        if (descMatch.Success)
        {
            tool.Description = descMatch.Groups[1].Value.Trim();
        }

        var paramsMatch = Regex.Match(content, @"###\s*Parameters\s*\n\n\|.*?\n\|[-:| ]+\n((?:\|.*?\n)+)", RegexOptions.Singleline);
        if (paramsMatch.Success)
        {
            var paramRows = paramsMatch.Groups[1].Value.Trim().Split('\n');
            foreach (var row in paramRows)
            {
                var param = ParseParameterRow(row);
                if (param != null)
                    tool.Parameters.Add(param);
            }
        }

        var responseMatch = Regex.Match(content, @"###\s*Response\s*\n\n```(?:json)?\n(.*?)\n```", RegexOptions.Singleline);
        if (responseMatch.Success)
        {
            tool.ResponseExample = responseMatch.Groups[1].Value.Trim();
        }

        return tool;
    }

    private ToolParameter? ParseParameterRow(string row)
    {
        var cells = row.Split('|')
            .Select(c => c.Trim())
            .Where(c => !string.IsNullOrEmpty(c))
            .ToList();

        if (cells.Count < 2) return null;

        return new ToolParameter
        {
            Name = cells[0],
            Type = cells.Count > 1 ? cells[1] : "string",
            Required = cells.Count > 2 && cells[2].Equals("Yes", StringComparison.OrdinalIgnoreCase),
            DefaultValue = cells.Count > 3 ? cells[3] : null,
            Description = cells.Count > 4 ? cells[4] : null
        };
    }

    private static string GetDefaultDocsPath()
    {
        var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
        var searchPaths = new[]
        {
            Path.Combine(assemblyPath, "tools", "mcp"),
            Path.Combine(assemblyPath, "..", "tools", "mcp"),
            Path.Combine(assemblyPath, "..", "..", "tools", "mcp"),
            Path.Combine(Directory.GetCurrentDirectory(), "tools", "mcp"),
        };

        foreach (var path in searchPaths)
        {
            if (Directory.Exists(path))
                return Path.GetFullPath(path);
        }

        return Path.Combine(assemblyPath, "tools", "mcp");
    }

    #endregion
}

#region Models

/// <summary>
/// Document metadata from frontmatter.
/// </summary>
public class DocumentMetadata
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Domain { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string Description { get; set; } = "";
}

/// <summary>
/// Documentation for a single tool.
/// </summary>
public class ToolDocumentation
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Version { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public List<ToolParameter> Parameters { get; set; } = new();
    public string? ResponseExample { get; set; }
    public string? SourceFile { get; set; }

    public ToolParameter? GetParameter(string name)
        => Parameters.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<ToolParameter> GetRequiredParameters()
        => Parameters.Where(p => p.Required);

    public IEnumerable<ToolParameter> GetOptionalParameters()
        => Parameters.Where(p => !p.Required);
}

/// <summary>
/// Parameter definition.
/// </summary>
public class ToolParameter
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "string";
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Documentation validation result.
/// </summary>
public class DocumentationValidationResult
{
    public List<string> UndocumentedTools { get; set; } = new();
    public List<string> OrphanedDocs { get; set; } = new();
    public int TotalTools { get; set; }
    public int DocumentedTools { get; set; }

    public bool IsValid => UndocumentedTools.Count == 0;
    public double CoveragePercent => TotalTools > 0 ? (DocumentedTools * 100.0 / TotalTools) : 0;

    public override string ToString()
    {
        return $"Documentation Coverage: {DocumentedTools}/{TotalTools} ({CoveragePercent:F1}%)\n" +
               $"Undocumented: {UndocumentedTools.Count}, Orphaned: {OrphanedDocs.Count}";
    }
}

#endregion
