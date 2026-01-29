namespace Served.SDK.Testing.Actions;

/// <summary>
/// Helper class for task-related test actions in Served.
/// </summary>
public class TaskActions
{
    private readonly ServedTestClient _client;

    public TaskActions(ServedTestClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Navigates to the tasks list.
    /// </summary>
    public async Task NavigateToTasksAsync()
    {
        await _client.NavigateAsync($"{_client.Config.BaseUrl}/tasks");
        await _client.WaitForReadyAsync();
    }

    /// <summary>
    /// Navigates to a specific task by ID.
    /// </summary>
    public async Task NavigateToTaskAsync(int taskId)
    {
        await _client.NavigateAsync($"{_client.Config.BaseUrl}/tasks/{taskId}");
        await _client.WaitForReadyAsync();
    }

    /// <summary>
    /// Searches for a task by name or ID.
    /// </summary>
    public async Task<bool> SearchTaskAsync(string query)
    {
        // Look for search input
        var searchSelectors = new[]
        {
            "[data-testid='task-search']",
            "input[placeholder*='Search']",
            ".task-search input",
            "[type='search']"
        };

        foreach (var selector in searchSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.FillAsync(selector, query);
                    await _client.WaitForReadyAsync();
                    return true;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        return false;
    }

    /// <summary>
    /// Clicks on a task in the list by its title.
    /// </summary>
    public async Task<bool> SelectTaskByTitleAsync(string title)
    {
        var taskSelector = $"[data-testid='task-item']:has-text('{title}'), .task-item:has-text('{title}'), tr:has-text('{title}')";

        try
        {
            await _client.WaitForSelectorAsync(taskSelector);
            await _client.ClickAsync(taskSelector);
            await _client.WaitForReadyAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a comment on the current task.
    /// </summary>
    public async Task<bool> AddCommentAsync(string comment)
    {
        // Look for comment input
        var commentSelectors = new[]
        {
            "[data-testid='comment-input']",
            "textarea[placeholder*='comment']",
            ".comment-input textarea",
            "[data-testid='add-comment'] textarea"
        };

        foreach (var selector in commentSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.FillAsync(selector, comment);

                    // Find and click submit button
                    var submitSelectors = new[]
                    {
                        "[data-testid='submit-comment']",
                        "button:has-text('Add Comment')",
                        "button:has-text('Submit')",
                        ".comment-submit"
                    };

                    foreach (var submitSelector in submitSelectors)
                    {
                        try
                        {
                            if (await _client.IsVisibleAsync(submitSelector))
                            {
                                await _client.ClickAsync(submitSelector);
                                await _client.WaitForReadyAsync();
                                return true;
                            }
                        }
                        catch
                        {
                            // Try next selector
                        }
                    }
                }
            }
            catch
            {
                // Try next selector
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the current task's title.
    /// </summary>
    public async Task<string?> GetCurrentTaskTitleAsync()
    {
        var titleSelectors = new[]
        {
            "[data-testid='task-title']",
            ".task-title",
            "h1.task-name",
            ".task-detail h1"
        };

        foreach (var selector in titleSelectors)
        {
            try
            {
                var text = await _client.GetTextAsync(selector);
                if (!string.IsNullOrEmpty(text))
                {
                    return text.Trim();
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
    /// Gets the current task's status.
    /// </summary>
    public async Task<string?> GetCurrentTaskStatusAsync()
    {
        var statusSelectors = new[]
        {
            "[data-testid='task-status']",
            ".task-status",
            ".status-badge"
        };

        foreach (var selector in statusSelectors)
        {
            try
            {
                var text = await _client.GetTextAsync(selector);
                if (!string.IsNullOrEmpty(text))
                {
                    return text.Trim();
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
    /// Changes the task status.
    /// </summary>
    public async Task<bool> ChangeStatusAsync(string newStatus)
    {
        // Click on status selector
        var statusSelectors = new[]
        {
            "[data-testid='task-status']",
            ".task-status",
            ".status-dropdown"
        };

        foreach (var selector in statusSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.ClickAsync(selector);
                    await _client.WaitForReadyAsync();

                    // Select the new status
                    var optionSelector = $"[data-status='{newStatus}'], .status-option:has-text('{newStatus}')";
                    await _client.ClickAsync(optionSelector);
                    await _client.WaitForReadyAsync();
                    return true;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        return false;
    }

    /// <summary>
    /// Assigns the task to a user.
    /// </summary>
    public async Task<bool> AssignToAsync(string userName)
    {
        var assigneeSelectors = new[]
        {
            "[data-testid='assignee-selector']",
            ".assignee-dropdown",
            "[data-testid='assign-button']"
        };

        foreach (var selector in assigneeSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.ClickAsync(selector);
                    await _client.WaitForReadyAsync();

                    // Select the user
                    var userSelector = $"[data-user='{userName}'], .user-option:has-text('{userName}')";
                    await _client.ClickAsync(userSelector);
                    await _client.WaitForReadyAsync();
                    return true;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        return false;
    }
}
