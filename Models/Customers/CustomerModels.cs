using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Served.SDK.Models.Customers;

#region Enums

/// <summary>
/// Customer type enumeration.
/// </summary>
public enum CustomerType
{
    /// <summary>Individual/person customer.</summary>
    Individual = 0,
    /// <summary>Company/business customer.</summary>
    Company = 1
}

#endregion

#region Response Wrappers

/// <summary>
/// Generic list response for customer queries.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class CustomerListResponse<T>
{
    /// <summary>The data items.</summary>
    [JsonProperty("data")]
    public List<T> Data { get; set; } = new();

    /// <summary>Total count of items (for pagination).</summary>
    [JsonProperty("totalCount")]
    public int? TotalCount { get; set; }
}

#endregion

#region View Models

/// <summary>
/// Summary view of a customer for listings.
/// </summary>
public class CustomerSummary
{
    /// <summary>Customer ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Customer name (company name or full name).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Customer number.</summary>
    [JsonProperty("customerNo")]
    public string? CustomerNo { get; set; }

    /// <summary>Primary email address.</summary>
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <summary>Primary phone number.</summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <summary>Whether customer is active.</summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Customer type.</summary>
    [JsonProperty("customerType")]
    public CustomerType CustomerType { get; set; }
}

/// <summary>
/// Detailed customer view with all fields.
/// </summary>
public class CustomerDetail
{
    /// <summary>Customer ID.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>Tenant ID.</summary>
    [JsonProperty("tenantId")]
    public int TenantId { get; set; }

    /// <summary>Customer name (company name).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Customer number.</summary>
    [JsonProperty("customerNo")]
    public string? CustomerNo { get; set; }

    /// <summary>Customer type.</summary>
    [JsonProperty("customerType")]
    public CustomerType CustomerType { get; set; }

    /// <summary>First name (for individuals).</summary>
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    /// <summary>Last name (for individuals).</summary>
    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    /// <summary>Primary email address.</summary>
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <summary>Primary phone number.</summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <summary>Website URL.</summary>
    [JsonProperty("website")]
    public string? Website { get; set; }

    /// <summary>VAT/CVR number.</summary>
    [JsonProperty("vatNumber")]
    public string? VatNumber { get; set; }

    /// <summary>Address line 1.</summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>Address line 2.</summary>
    [JsonProperty("address2")]
    public string? Address2 { get; set; }

    /// <summary>City.</summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>Postal/ZIP code.</summary>
    [JsonProperty("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>Country.</summary>
    [JsonProperty("country")]
    public string? Country { get; set; }

    /// <summary>Whether customer is active.</summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Notes about the customer.</summary>
    [JsonProperty("notes")]
    public string? Notes { get; set; }

    /// <summary>Payment terms in days.</summary>
    [JsonProperty("paymentTermsDays")]
    public int? PaymentTermsDays { get; set; }

    /// <summary>Default hourly rate.</summary>
    [JsonProperty("defaultHourlyRate")]
    public decimal? DefaultHourlyRate { get; set; }

    /// <summary>Currency code (e.g., DKK, EUR).</summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>Created date.</summary>
    [JsonProperty("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>Updated date.</summary>
    [JsonProperty("updatedDate")]
    public DateTime? UpdatedDate { get; set; }

    /// <summary>Created by user ID.</summary>
    [JsonProperty("createdBy")]
    public int CreatedBy { get; set; }

    /// <summary>Updated by user ID.</summary>
    [JsonProperty("updatedBy")]
    public int? UpdatedBy { get; set; }
}

#endregion

#region Request Models

/// <summary>
/// Request to create a new customer.
/// </summary>
public class CreateCustomerRequest
{
    /// <summary>Customer name (required).</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Customer type.</summary>
    [JsonProperty("customerType")]
    public CustomerType CustomerType { get; set; } = CustomerType.Company;

    /// <summary>First name (for individuals).</summary>
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    /// <summary>Last name (for individuals).</summary>
    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    /// <summary>Primary email address.</summary>
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <summary>Primary phone number.</summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <summary>Website URL.</summary>
    [JsonProperty("website")]
    public string? Website { get; set; }

    /// <summary>VAT/CVR number.</summary>
    [JsonProperty("vatNumber")]
    public string? VatNumber { get; set; }

    /// <summary>Address line 1.</summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>City.</summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>Postal/ZIP code.</summary>
    [JsonProperty("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>Country.</summary>
    [JsonProperty("country")]
    public string? Country { get; set; }

    /// <summary>Notes about the customer.</summary>
    [JsonProperty("notes")]
    public string? Notes { get; set; }

    /// <summary>Payment terms in days.</summary>
    [JsonProperty("paymentTermsDays")]
    public int? PaymentTermsDays { get; set; }

    /// <summary>Default hourly rate.</summary>
    [JsonProperty("defaultHourlyRate")]
    public decimal? DefaultHourlyRate { get; set; }

    /// <summary>Currency code (e.g., DKK, EUR).</summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// Request to update an existing customer.
/// Note: ID is passed separately to the update method.
/// </summary>
public class UpdateCustomerRequest
{
    /// <summary>Customer name.</summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>First name (for individuals).</summary>
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    /// <summary>Last name (for individuals).</summary>
    [JsonProperty("lastName")]
    public string? LastName { get; set; }

    /// <summary>Primary email address.</summary>
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <summary>Primary phone number.</summary>
    [JsonProperty("phone")]
    public string? Phone { get; set; }

    /// <summary>Website URL.</summary>
    [JsonProperty("website")]
    public string? Website { get; set; }

    /// <summary>VAT/CVR number.</summary>
    [JsonProperty("vatNumber")]
    public string? VatNumber { get; set; }

    /// <summary>Address line 1.</summary>
    [JsonProperty("address")]
    public string? Address { get; set; }

    /// <summary>City.</summary>
    [JsonProperty("city")]
    public string? City { get; set; }

    /// <summary>Postal/ZIP code.</summary>
    [JsonProperty("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>Country.</summary>
    [JsonProperty("country")]
    public string? Country { get; set; }

    /// <summary>Whether customer is active.</summary>
    [JsonProperty("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>Notes about the customer.</summary>
    [JsonProperty("notes")]
    public string? Notes { get; set; }

    /// <summary>Payment terms in days.</summary>
    [JsonProperty("paymentTermsDays")]
    public int? PaymentTermsDays { get; set; }

    /// <summary>Default hourly rate.</summary>
    [JsonProperty("defaultHourlyRate")]
    public decimal? DefaultHourlyRate { get; set; }

    /// <summary>Currency code (e.g., DKK, EUR).</summary>
    [JsonProperty("currency")]
    public string? Currency { get; set; }

    /// <summary>Row version for optimistic concurrency.</summary>
    [JsonProperty("rowVersion")]
    public byte[]? RowVersion { get; set; }
}

/// <summary>
/// Query parameters for filtering and paginating customers.
/// </summary>
public class CustomerQueryParams : Common.QueryParams
{
    /// <summary>Active status filter.</summary>
    [JsonProperty("isActive")]
    public bool? IsActive { get; set; } = true;

    /// <summary>Customer type filter.</summary>
    [JsonProperty("customerType")]
    public CustomerType? CustomerType { get; set; }

    /// <summary>Customer type ID filter.</summary>
    [JsonProperty("customerTypeId")]
    public int? CustomerTypeId { get; set; }

    /// <summary>Field to sort by.</summary>
    [JsonProperty("sortBy")]
    public string SortBy { get; set; } = "name";

    /// <summary>Sort direction (asc/desc).</summary>
    [JsonProperty("sortDirection")]
    public string SortDirection { get; set; } = "asc";

    /// <summary>Include total count in response.</summary>
    [JsonProperty("includeTotalCount")]
    public bool IncludeTotalCount { get; set; } = true;
}

#endregion

#region Bulk Operations

/// <summary>
/// Request to bulk create customers.
/// </summary>
public class BulkCreateCustomersRequest
{
    /// <summary>List of customers to create.</summary>
    [JsonProperty("customers")]
    public List<CreateCustomerRequest> Customers { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Item for bulk customer update.
/// </summary>
public class BulkUpdateCustomerItem
{
    /// <summary>Customer ID to update.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Update data.</summary>
    [JsonProperty("data")]
    public UpdateCustomerRequest Data { get; set; } = new();
}

/// <summary>
/// Request to bulk update customers.
/// </summary>
public class BulkUpdateCustomersRequest
{
    /// <summary>List of customer updates.</summary>
    [JsonProperty("customers")]
    public List<BulkUpdateCustomerItem> Customers { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// Request to bulk delete customers.
/// </summary>
public class BulkDeleteCustomersRequest
{
    /// <summary>List of customer IDs to delete.</summary>
    [JsonProperty("ids")]
    public List<int> Ids { get; set; } = new();

    /// <summary>Continue on error.</summary>
    [JsonProperty("continueOnError")]
    public bool ContinueOnError { get; set; } = true;
}

#endregion

