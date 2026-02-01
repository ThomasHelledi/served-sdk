# Customers API

Manage customers (clients/companies) in your workspace.

Access via: `client.Companies.Customers`

---

## Methods

### GetAllAsync

Get all customers with optional filtering.

```csharp
var customers = await client.Companies.Customers.GetAllAsync();

// With filtering
var activeCustomers = await client.Companies.Customers.GetAllAsync(
    new CustomerQueryParams
    {
        Status = "active",
        Take = 50
    }
);
```

**Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `Status` | string | Filter by status |
| `Take` | int | Max results (default: 100) |
| `Skip` | int | Offset for pagination |

**Returns:** `List<CustomerSummary>`

---

### GetAsync

Get a single customer by ID.

```csharp
var customer = await client.Companies.Customers.GetAsync(123);

Console.WriteLine($"Customer: {customer.Name}");
Console.WriteLine($"Email: {customer.Email}");
Console.WriteLine($"Phone: {customer.Phone}");
```

**Returns:** `CustomerDetail`

---

### CreateAsync

Create a new customer.

```csharp
var customer = await client.Companies.Customers.CreateAsync(
    new CreateCustomerRequest
    {
        Name = "Acme Corporation",
        Email = "contact@acme.com",
        Phone = "+45 12 34 56 78",
        Address = "123 Main Street",
        City = "Copenhagen",
        Country = "Denmark",
        VatNumber = "DK12345678"
    }
);

Console.WriteLine($"Created customer #{customer.Id}");
```

**Request Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `Name` | string | Yes | Customer/company name |
| `Email` | string | No | Primary email |
| `Phone` | string | No | Phone number |
| `Address` | string | No | Street address |
| `City` | string | No | City |
| `Country` | string | No | Country |
| `VatNumber` | string | No | VAT/tax number |

---

### UpdateAsync

Update an existing customer.

```csharp
var updated = await client.Companies.Customers.UpdateAsync(
    123,
    new UpdateCustomerRequest
    {
        Email = "new-contact@acme.com",
        Phone = "+45 98 76 54 32"
    }
);
```

---

### DeleteAsync

Delete a customer.

```csharp
await client.Companies.Customers.DeleteAsync(123);
```

> **Warning:** Consider the impact on related projects and invoices.

---

## Bulk Operations

### CreateBulkAsync

Create multiple customers at once.

```csharp
var result = await client.Companies.Customers.CreateBulkAsync(
    new BulkCreateCustomersRequest
    {
        Customers = new List<CreateCustomerRequest>
        {
            new() { Name = "Customer A", Email = "a@example.com" },
            new() { Name = "Customer B", Email = "b@example.com" },
            new() { Name = "Customer C", Email = "c@example.com" }
        }
    }
);

Console.WriteLine($"Created: {result.Succeeded.Count}");
```

---

## Query Operations

### SearchAsync

Search customers by name or email.

```csharp
var results = await client.Companies.Customers.SearchAsync(
    "acme",
    take: 10
);
```

### GetRangeAsync

Get multiple customers by their IDs.

```csharp
var customers = await client.Companies.Customers.GetRangeAsync(
    new List<int> { 1, 2, 3, 4, 5 }
);
```

---

## Models

### CustomerSummary

```csharp
public class CustomerSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int ProjectCount { get; set; }
}
```

### CustomerDetail

```csharp
public class CustomerDetail : CustomerSummary
{
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string VatNumber { get; set; }
    public string Website { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## Common Patterns

### Customer with Projects

```csharp
// Get customer and their projects
var customer = await client.Companies.Customers.GetAsync(customerId);
var projects = await client.ProjectManagement.Projects.GetAllAsync(
    new ProjectQueryParams { CustomerId = customerId }
);

Console.WriteLine($"{customer.Name} has {projects.Count} projects");
```

### Customer Search Autocomplete

```csharp
public async Task<List<CustomerSummary>> SearchCustomersAsync(string term)
{
    if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        return new List<CustomerSummary>();

    return await _client.Companies.Customers.SearchAsync(term, take: 10);
}
```
