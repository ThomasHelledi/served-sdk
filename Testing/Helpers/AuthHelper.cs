using Microsoft.Playwright;

namespace Served.SDK.Testing.Helpers;

/// <summary>
/// Helper class for authentication flows in Served tests.
/// </summary>
public class AuthHelper
{
    private readonly ServedTestClient _client;

    public AuthHelper(ServedTestClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Performs login with the configured credentials.
    /// </summary>
    public async Task<bool> LoginAsync()
    {
        var credentials = _client.Config.Credentials;
        if (credentials == null)
        {
            throw new InvalidOperationException("No credentials configured");
        }

        return await LoginAsync(credentials.Username, credentials.Password);
    }

    /// <summary>
    /// Performs login with the specified credentials.
    /// </summary>
    public async Task<bool> LoginAsync(string username, string password)
    {
        await _client.NavigateAsync($"{_client.Config.BaseUrl}/login");
        await _client.WaitForReadyAsync();

        // Fill login form
        await _client.FillAsync("[data-testid='username'], input[name='username'], input[type='email']", username);
        await _client.FillAsync("[data-testid='password'], input[name='password'], input[type='password']", password);

        // Click login button
        await _client.ClickAsync("[data-testid='login-button'], button[type='submit']");

        // Wait for navigation
        await _client.WaitForReadyAsync();

        // Check if login was successful (not on login page anymore)
        var currentUrl = _client.GetUrl();
        return !currentUrl.Contains("/login");
    }

    /// <summary>
    /// Performs logout.
    /// </summary>
    public async Task LogoutAsync()
    {
        // Try common logout patterns
        var logoutSelectors = new[]
        {
            "[data-testid='logout']",
            "a[href='/logout']",
            "button:has-text('Logout')",
            "button:has-text('Log out')",
            ".logout-button"
        };

        foreach (var selector in logoutSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
                    await _client.ClickAsync(selector);
                    await _client.WaitForReadyAsync();
                    return;
                }
            }
            catch
            {
                // Try next selector
            }
        }

        // Fallback: navigate to logout URL
        await _client.NavigateAsync($"{_client.Config.BaseUrl}/logout");
    }

    /// <summary>
    /// Checks if the user is currently logged in.
    /// </summary>
    public async Task<bool> IsLoggedInAsync()
    {
        var currentUrl = _client.GetUrl();

        // Check URL doesn't indicate login page
        if (currentUrl.Contains("/login") || currentUrl.Contains("/auth"))
        {
            return false;
        }

        // Check for authenticated elements
        var authenticatedSelectors = new[]
        {
            "[data-testid='user-menu']",
            ".user-avatar",
            "[data-testid='dashboard']",
            "nav .user-info"
        };

        foreach (var selector in authenticatedSelectors)
        {
            try
            {
                if (await _client.IsVisibleAsync(selector))
                {
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
    /// Ensures the user is logged in, performing login if necessary.
    /// </summary>
    public async Task EnsureLoggedInAsync()
    {
        if (!await IsLoggedInAsync())
        {
            await LoginAsync();
        }
    }

    /// <summary>
    /// Saves authentication state for session persistence.
    /// </summary>
    public async Task SaveAuthStateAsync(string path)
    {
        if (_client.Context == null)
        {
            throw new InvalidOperationException("Browser context not initialized");
        }

        await _client.Context.StorageStateAsync(new BrowserContextStorageStateOptions
        {
            Path = path
        });
    }

    /// <summary>
    /// Selects a tenant for multi-tenant authentication.
    /// </summary>
    public async Task SelectTenantAsync(int tenantId)
    {
        // Look for tenant selector
        var tenantSelector = $"[data-tenant-id='{tenantId}'], [data-testid='tenant-{tenantId}']";

        if (await _client.IsVisibleAsync(tenantSelector))
        {
            await _client.ClickAsync(tenantSelector);
            await _client.WaitForReadyAsync();
        }
    }
}
