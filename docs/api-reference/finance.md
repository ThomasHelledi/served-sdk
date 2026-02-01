# Finance API

Access financial data including invoices.

Access via: `client.FinanceModule.Invoices`

---

## Methods

### GetInvoicesAsync

Get a list of invoices.

```csharp
var invoices = await client.FinanceModule.Invoices.GetInvoicesAsync();

// With limit
var recentInvoices = await client.FinanceModule.Invoices.GetInvoicesAsync(
    limit: 50
);

foreach (var invoice in invoices)
{
    Console.WriteLine($"Invoice #{invoice.Number}: {invoice.Total:C}");
}
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `limit` | int | Max results (default: 20) |

**Returns:** `List<InvoiceViewModel>`

---

## Models

### InvoiceViewModel

```csharp
public class InvoiceViewModel
{
    public int Id { get; set; }
    public string Number { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; }
}
```

### Invoice Statuses

| Status | Description |
|--------|-------------|
| `draft` | Not yet sent |
| `sent` | Sent to customer |
| `paid` | Payment received |
| `overdue` | Past due date |
| `cancelled` | Cancelled |

---

## Common Patterns

### Outstanding Invoices

```csharp
public async Task<decimal> GetOutstandingAmountAsync()
{
    var invoices = await _client.FinanceModule.Invoices.GetInvoicesAsync(100);

    return invoices
        .Where(i => i.Status == "sent" || i.Status == "overdue")
        .Sum(i => i.Total);
}
```

### Invoices by Customer

```csharp
public async Task<List<InvoiceViewModel>> GetCustomerInvoicesAsync(int customerId)
{
    var allInvoices = await _client.FinanceModule.Invoices.GetInvoicesAsync(100);

    return allInvoices
        .Where(i => i.CustomerId == customerId)
        .OrderByDescending(i => i.InvoiceDate)
        .ToList();
}
```

### Revenue Summary

```csharp
public async Task<RevenueSummary> GetRevenueSummaryAsync()
{
    var invoices = await _client.FinanceModule.Invoices.GetInvoicesAsync(100);

    return new RevenueSummary
    {
        TotalInvoiced = invoices.Sum(i => i.Total),
        PaidAmount = invoices
            .Where(i => i.Status == "paid")
            .Sum(i => i.Total),
        OutstandingAmount = invoices
            .Where(i => i.Status == "sent" || i.Status == "overdue")
            .Sum(i => i.Total),
        OverdueAmount = invoices
            .Where(i => i.Status == "overdue")
            .Sum(i => i.Total)
    };
}
```

---

## Integration Notes

The Finance API provides read access to invoice data. For full invoice management (create, send, payment recording), use the UnifiedHQ web interface or integrate with your accounting system.

### Supported Accounting Integrations

- **E-conomic** (Denmark)
- **Dinero** (Denmark)
- **QuickBooks** (International)

Configure integrations at [unifiedhq.ai/app/settings/integrations](https://unifiedhq.ai/app/settings/integrations)
