using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Served.SDK.Models.Finance;

public class InvoiceViewModel
{
    public Guid Id { get; set; }
    public string? InvoiceNo { get; set; }
    public DateTime InvoiceDate { get; set; }
    public double Amount { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public InvoiceStatusEnum InvoiceStatus { get; set; } 
    public int? CustomerId { get; set; }
    public int? ProjectId { get; set; }
    public string? Header { get; set; }
    public DateTime? DueDate { get; set; }
    
    public string StatusName => InvoiceStatus.ToString();
}

public enum InvoiceStatusEnum
{
    Working = 0,
    Booked = 1,
    Overdue = 2,
    Paid = 3,
    Draft = 4,
    Sent = 5
}

public class InvoiceGroupingQueryParams
{
    public string? Search { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}
