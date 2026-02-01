# Authentication

The Served.SDK supports multiple authentication methods. This guide covers all available options.

---

## API Key Authentication (Recommended)

The simplest and most common method for server-to-server communication.

### Getting an API Key

1. Log in to [unifiedhq.ai](https://unifiedhq.ai)
2. Navigate to **Settings** > **API Keys**
3. Click **Create API Key**
4. Select required scopes
5. Copy and store securely

### Using API Key

```csharp
var client = new ServedClientBuilder()
    .WithApiKey("sk_live_xxxxxxxxxxxxx")
    .WithTenant("your-workspace")
    .Build();
```

### API Key Best Practices

- Never commit API keys to source control
- Use environment variables or secret managers
- Rotate keys periodically
- Use minimum required scopes

---

## JWT Token Authentication

For applications that already have a JWT token from OAuth flow.

```csharp
var client = new ServedClientBuilder()
    .WithBearerToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
    .WithTenant("your-workspace")
    .Build();
```

---

## OAuth 2.0

For applications that need to act on behalf of users.

### Authorization Code Flow

```csharp
// 1. Redirect user to authorization URL
var authUrl = "https://unifiedhq.ai/oauth/authorize" +
    "?client_id=YOUR_CLIENT_ID" +
    "&redirect_uri=YOUR_CALLBACK_URL" +
    "&response_type=code" +
    "&scope=projects tasks timetracking";

// 2. Exchange code for token (in callback)
var tokenResponse = await ExchangeCodeForToken(code);

// 3. Use token with SDK
var client = new ServedClientBuilder()
    .WithBearerToken(tokenResponse.AccessToken)
    .WithTenant("user-workspace")
    .Build();
```

### Available Scopes

| Scope | Description |
|-------|-------------|
| `projects` | Read/write projects |
| `tasks` | Read/write tasks |
| `timetracking` | Read/write time registrations |
| `customers` | Read/write customers |
| `employees` | Read employees |
| `calendar` | Read/write agreements |
| `finance` | Read/write invoices |
| `devops` | Read DevOps data |

---

## Environment Configuration

### Using Environment Variables

```csharp
var client = new ServedClientBuilder()
    .WithApiKey(Environment.GetEnvironmentVariable("SERVED_API_KEY"))
    .WithTenant(Environment.GetEnvironmentVariable("SERVED_TENANT"))
    .Build();
```

### Using Configuration Files

```json
// appsettings.json
{
  "Served": {
    "ApiKey": "sk_live_xxxxxxxxxxxxx",
    "Tenant": "your-workspace",
    "BaseUrl": "https://apis.unifiedhq.ai"
  }
}
```

```csharp
// Program.cs
builder.Services.AddServedClient(
    builder.Configuration.GetSection("Served")
);
```

---

## Multi-Tenant Access

Switch between workspaces dynamically:

```csharp
// Get tenant ID from slug
var tenantId = await client.GetTenantIdAsync("other-workspace");

// Make requests with different tenant context
var projects = await client.ProjectManagement.Projects.GetAllAsync(
    new ProjectQueryParams { TenantId = tenantId }
);
```

---

## Token Refresh

For long-running applications using OAuth:

```csharp
public class TokenRefreshHandler
{
    private readonly IServedClient _client;
    private string _refreshToken;

    public async Task<string> GetValidTokenAsync()
    {
        if (IsTokenExpired())
        {
            var newToken = await RefreshTokenAsync(_refreshToken);
            _refreshToken = newToken.RefreshToken;
            return newToken.AccessToken;
        }
        return _currentToken;
    }
}
```

---

## Security Recommendations

1. **Never expose API keys in client-side code**
2. **Use HTTPS only** (enforced by default)
3. **Implement proper secret management**
4. **Monitor API key usage** in UnifiedHQ dashboard
5. **Revoke compromised keys immediately**

---

## Troubleshooting

### 401 Unauthorized

- Verify API key is correct and active
- Check that tenant slug matches your workspace
- Ensure required scopes are granted

### 403 Forbidden

- API key may lack required scopes
- Resource may belong to different tenant

### Rate Limiting

The API enforces rate limits. Handle with exponential backoff:

```csharp
var client = new ServedClientBuilder()
    .WithApiKey("your-key")
    .WithRetry(3)  // Automatic retry with backoff
    .Build();
```
