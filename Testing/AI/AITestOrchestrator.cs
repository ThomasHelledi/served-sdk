using System.Text.Json;

namespace Served.SDK.Testing.AI;

/// <summary>
/// AI-powered test orchestrator for intelligent test flows.
/// Uses AI to make decisions during test execution.
/// </summary>
public class AITestOrchestrator
{
    private readonly ServedTestClient _client;
    private readonly AITestConfiguration _config;
    private readonly List<AIDecision> _decisions = new();

    public AITestOrchestrator(ServedTestClient client, AITestConfiguration? config = null)
    {
        _client = client;
        _config = config ?? new AITestConfiguration();
    }

    /// <summary>
    /// Gets all AI decisions made during the test.
    /// </summary>
    public IReadOnlyList<AIDecision> Decisions => _decisions.AsReadOnly();

    /// <summary>
    /// Executes an AI-driven test flow.
    /// </summary>
    public async Task<AITestResult> ExecuteFlowAsync(AITestFlow flow)
    {
        var result = new AITestResult
        {
            FlowName = flow.Name,
            StartedAt = DateTime.UtcNow
        };

        try
        {
            foreach (var step in flow.Steps)
            {
                var stepResult = await ExecuteStepAsync(step);
                result.StepResults.Add(stepResult);

                if (!stepResult.Success && !step.ContinueOnFailure)
                {
                    result.Status = AITestStatus.Failed;
                    result.Error = stepResult.Error;
                    break;
                }
            }

            if (result.Status != AITestStatus.Failed)
            {
                result.Status = AITestStatus.Passed;
            }
        }
        catch (Exception ex)
        {
            result.Status = AITestStatus.Failed;
            result.Error = ex.Message;
        }
        finally
        {
            result.CompletedAt = DateTime.UtcNow;
            result.Decisions = _decisions.ToList();
        }

        return result;
    }

    /// <summary>
    /// Executes a single step with AI decision support.
    /// </summary>
    private async Task<AIStepResult> ExecuteStepAsync(AITestStep step)
    {
        var stepResult = new AIStepResult
        {
            StepName = step.Name,
            StartedAt = DateTime.UtcNow
        };

        try
        {
            // Check preconditions
            if (step.Preconditions.Any())
            {
                foreach (var precondition in step.Preconditions)
                {
                    var satisfied = await EvaluatePreconditionAsync(precondition);
                    if (!satisfied && !step.SkipIfPreconditionFails)
                    {
                        stepResult.Success = false;
                        stepResult.Error = $"Precondition failed: {precondition}";
                        return stepResult;
                    }
                }
            }

            // Execute the action
            switch (step.Action)
            {
                case AIAction.Navigate:
                    await _client.NavigateAsync(step.Target);
                    break;

                case AIAction.Click:
                    await SmartClickAsync(step.Target, step.Fallbacks);
                    break;

                case AIAction.Fill:
                    await SmartFillAsync(step.Target, step.Value ?? "", step.Fallbacks);
                    break;

                case AIAction.Wait:
                    await _client.WaitForSelectorAsync(step.Target);
                    break;

                case AIAction.Verify:
                    stepResult.Success = await VerifyAsync(step.Target, step.ExpectedValue);
                    if (!stepResult.Success)
                    {
                        stepResult.Error = $"Verification failed for {step.Target}";
                    }
                    break;

                case AIAction.Decision:
                    var decision = await MakeDecisionAsync(step);
                    _decisions.Add(decision);
                    stepResult.Decision = decision;
                    break;

                case AIAction.ConditionalBranch:
                    await ExecuteConditionalBranchAsync(step);
                    break;

                default:
                    throw new NotSupportedException($"Action {step.Action} not supported");
            }

            stepResult.Success = true;
        }
        catch (Exception ex)
        {
            stepResult.Success = false;
            stepResult.Error = ex.Message;
        }
        finally
        {
            stepResult.CompletedAt = DateTime.UtcNow;
        }

        return stepResult;
    }

    /// <summary>
    /// Smart click that tries multiple selectors.
    /// </summary>
    private async Task SmartClickAsync(string primarySelector, List<string>? fallbacks = null)
    {
        var selectors = new List<string> { primarySelector };
        if (fallbacks != null)
        {
            selectors.AddRange(fallbacks);
        }

        foreach (var selector in selectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.ClickAsync(selector);
                    return;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        throw new Exception($"Could not find clickable element: {primarySelector}");
    }

    /// <summary>
    /// Smart fill that tries multiple selectors.
    /// </summary>
    private async Task SmartFillAsync(string primarySelector, string value, List<string>? fallbacks = null)
    {
        var selectors = new List<string> { primarySelector };
        if (fallbacks != null)
        {
            selectors.AddRange(fallbacks);
        }

        foreach (var selector in selectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.FillAsync(selector, value);
                    return;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        throw new Exception($"Could not find fillable element: {primarySelector}");
    }

    /// <summary>
    /// Verifies an element's value or visibility.
    /// </summary>
    private async Task<bool> VerifyAsync(string selector, string? expectedValue)
    {
        if (string.IsNullOrEmpty(expectedValue))
        {
            return await _client.IsVisibleAsync(selector);
        }

        var actualValue = await _client.GetTextAsync(selector);
        return actualValue?.Contains(expectedValue) ?? false;
    }

    /// <summary>
    /// Evaluates a precondition.
    /// </summary>
    private async Task<bool> EvaluatePreconditionAsync(string precondition)
    {
        // Simple precondition evaluation
        if (precondition.StartsWith("visible:"))
        {
            var selector = precondition.Substring(8);
            return await _client.IsVisibleAsync(selector);
        }

        if (precondition.StartsWith("url:"))
        {
            var expectedUrl = precondition.Substring(4);
            return _client.GetUrl().Contains(expectedUrl);
        }

        if (precondition.StartsWith("logged_in"))
        {
            return await _client.Auth.IsLoggedInAsync();
        }

        return true;
    }

    /// <summary>
    /// Makes an AI-driven decision.
    /// </summary>
    private async Task<AIDecision> MakeDecisionAsync(AITestStep step)
    {
        var decision = new AIDecision
        {
            Context = step.DecisionContext ?? "",
            Options = step.DecisionOptions ?? new List<string>(),
            Timestamp = DateTime.UtcNow
        };

        // Gather page context for decision
        var pageTitle = await _client.GetTitleAsync();
        var pageUrl = _client.GetUrl();

        // Simple rule-based decision (can be extended with actual AI)
        decision.SelectedOption = await EvaluateDecisionAsync(decision, pageTitle, pageUrl);
        decision.Reasoning = $"Selected based on page context: {pageTitle}";

        return decision;
    }

    /// <summary>
    /// Evaluates a decision based on context.
    /// </summary>
    private async Task<string> EvaluateDecisionAsync(AIDecision decision, string pageTitle, string pageUrl)
    {
        // Rule-based decision making (can be replaced with actual AI)
        foreach (var option in decision.Options)
        {
            // Check if option matches current page context
            if (pageTitle.Contains(option, StringComparison.OrdinalIgnoreCase) ||
                pageUrl.Contains(option, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        // Default to first option
        return decision.Options.FirstOrDefault() ?? "default";
    }

    /// <summary>
    /// Executes a conditional branch based on AI decision.
    /// </summary>
    private async Task ExecuteConditionalBranchAsync(AITestStep step)
    {
        var decision = await MakeDecisionAsync(step);
        _decisions.Add(decision);

        if (step.Branches != null && step.Branches.TryGetValue(decision.SelectedOption, out var branch))
        {
            foreach (var branchStep in branch)
            {
                await ExecuteStepAsync(branchStep);
            }
        }
    }
}

/// <summary>
/// Configuration for AI test orchestrator.
/// </summary>
public class AITestConfiguration
{
    public string? AIEndpoint { get; set; }
    public string? AIApiKey { get; set; }
    public bool UseRuleBased { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan DecisionTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>
/// Represents an AI-driven test flow.
/// </summary>
public class AITestFlow
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<AITestStep> Steps { get; set; } = new();
}

/// <summary>
/// Represents a step in an AI test flow.
/// </summary>
public class AITestStep
{
    public string Name { get; set; } = string.Empty;
    public AIAction Action { get; set; }
    public string Target { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? ExpectedValue { get; set; }
    public List<string>? Fallbacks { get; set; }
    public List<string> Preconditions { get; set; } = new();
    public bool ContinueOnFailure { get; set; }
    public bool SkipIfPreconditionFails { get; set; }

    // Decision properties
    public string? DecisionContext { get; set; }
    public List<string>? DecisionOptions { get; set; }
    public Dictionary<string, List<AITestStep>>? Branches { get; set; }
}

/// <summary>
/// Actions that can be performed in AI tests.
/// </summary>
public enum AIAction
{
    Navigate,
    Click,
    Fill,
    Wait,
    Verify,
    Decision,
    ConditionalBranch
}

/// <summary>
/// Result of an AI test flow.
/// </summary>
public class AITestResult
{
    public string FlowName { get; set; } = string.Empty;
    public AITestStatus Status { get; set; } = AITestStatus.Pending;
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? Error { get; set; }
    public List<AIStepResult> StepResults { get; set; } = new();
    public List<AIDecision> Decisions { get; set; } = new();

    public TimeSpan Duration => CompletedAt - StartedAt;
}

/// <summary>
/// Result of a single AI test step.
/// </summary>
public class AIStepResult
{
    public string StepName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? Error { get; set; }
    public AIDecision? Decision { get; set; }

    public TimeSpan Duration => CompletedAt - StartedAt;
}

/// <summary>
/// Represents an AI decision made during test execution.
/// </summary>
public class AIDecision
{
    public string Context { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public string SelectedOption { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Status of an AI test.
/// </summary>
public enum AITestStatus
{
    Pending,
    Running,
    Passed,
    Failed,
    Skipped
}
