using System;
using System.Collections.Generic;

namespace Served.SDK.Models.Sales;

#region Pipeline Models

/// <summary>
/// Represents a sales pipeline.
/// </summary>
public class PipelineViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int WorkspaceId { get; set; }
    public bool IsDefault { get; set; }
    public string Currency { get; set; } = "DKK";
    public bool IsActive { get; set; }
    public int? RottenDays { get; set; }
    public List<PipelineStageViewModel> Stages { get; set; } = new();
    public int DealCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Pipeline list item.
/// </summary>
public class PipelineListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsDefault { get; set; }
    public string Currency { get; set; } = "DKK";
    public int StageCount { get; set; }
    public int DealCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
}

/// <summary>
/// A stage in a pipeline.
/// </summary>
public class PipelineStageViewModel
{
    public int Id { get; set; }
    public int PipelineId { get; set; }
    public string Name { get; set; } = "";
    public int Probability { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsWon { get; set; }
    public bool IsLost { get; set; }
    public int? RottenDays { get; set; }
    public int DealCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
}

/// <summary>
/// Request to create a pipeline.
/// </summary>
public class CreatePipelineRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int WorkspaceId { get; set; }
    public bool IsDefault { get; set; }
    public string Currency { get; set; } = "DKK";
    public int? RottenDays { get; set; }
    public List<CreatePipelineStageRequest>? Stages { get; set; }
}

/// <summary>
/// Request to update a pipeline.
/// </summary>
public class UpdatePipelineRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsDefault { get; set; }
    public string? Currency { get; set; }
    public bool? IsActive { get; set; }
    public int? RottenDays { get; set; }
}

/// <summary>
/// Request to create a pipeline stage.
/// </summary>
public class CreatePipelineStageRequest
{
    public string Name { get; set; } = "";
    public int Probability { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
    public bool IsWon { get; set; }
    public bool IsLost { get; set; }
    public int? RottenDays { get; set; }
}

#endregion

#region Deal Models

/// <summary>
/// Represents a sales deal/opportunity.
/// </summary>
public class DealViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Value { get; set; }
    public string Currency { get; set; } = "DKK";
    public int? Probability { get; set; }
    public decimal WeightedValue { get; set; }

    // Relationships
    public int PipelineId { get; set; }
    public string? PipelineName { get; set; }
    public int StageId { get; set; }
    public string? StageName { get; set; }
    public string? StageColor { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }

    // Dates
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }

    // Status
    public string Status { get; set; } = "Open";
    public string? LostReason { get; set; }
    public string? WonNotes { get; set; }

    // Metadata
    public string? Source { get; set; }
    public List<string>? Tags { get; set; }

    // Computed
    public bool IsRotten { get; set; }
    public int? DaysInStage { get; set; }
}

/// <summary>
/// Deal list item.
/// </summary>
public class DealListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Value { get; set; }
    public string Currency { get; set; } = "DKK";
    public decimal WeightedValue { get; set; }
    public int StageId { get; set; }
    public string? StageName { get; set; }
    public string? CustomerName { get; set; }
    public string? ContactName { get; set; }
    public string? OwnerName { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string Status { get; set; } = "Open";
    public bool IsRotten { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// Request to create a deal.
/// </summary>
public class CreateDealRequest
{
    public string Name { get; set; } = "";
    public decimal Value { get; set; }
    public string Currency { get; set; } = "DKK";
    public int? Probability { get; set; }
    public int PipelineId { get; set; }
    public int StageId { get; set; }
    public int? CustomerId { get; set; }
    public int? ContactId { get; set; }
    public int? OwnerId { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? Source { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Request to update a deal.
/// </summary>
public class UpdateDealRequest
{
    public string? Name { get; set; }
    public decimal? Value { get; set; }
    public string? Currency { get; set; }
    public int? Probability { get; set; }
    public int? CustomerId { get; set; }
    public int? ContactId { get; set; }
    public int? OwnerId { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? Source { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Request to search for deals.
/// </summary>
public class DealSearchRequest
{
    public int? PipelineId { get; set; }
    public int? StageId { get; set; }
    public int? OwnerId { get; set; }
    public int? CustomerId { get; set; }
    public string? Status { get; set; }
    public string? Query { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public bool? IsRotten { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDesc { get; set; } = true;
}

/// <summary>
/// Request to move a deal to another stage.
/// </summary>
public class MoveDealRequest
{
    public int StageId { get; set; }
}

/// <summary>
/// Request to mark a deal as won.
/// </summary>
public class WonDealRequest
{
    public string? Notes { get; set; }
    public DateTime? ActualCloseDate { get; set; }
}

/// <summary>
/// Request to mark a deal as lost.
/// </summary>
public class LostDealRequest
{
    public string Reason { get; set; } = "";
}

#endregion

#region Analytics Models

/// <summary>
/// Pipeline analytics data.
/// </summary>
public class PipelineAnalyticsViewModel
{
    public int PipelineId { get; set; }
    public string PipelineName { get; set; } = "";
    public int TotalDeals { get; set; }
    public decimal TotalValue { get; set; }
    public decimal WeightedValue { get; set; }
    public int OpenDeals { get; set; }
    public int WonDeals { get; set; }
    public int LostDeals { get; set; }
    public decimal WinRate { get; set; }
    public double AverageDaysToWin { get; set; }
    public double AverageDaysInPipeline { get; set; }
}

/// <summary>
/// Sales forecast data.
/// </summary>
public class SalesForecastViewModel
{
    public string Period { get; set; } = "";
    public decimal Committed { get; set; }
    public decimal BestCase { get; set; }
    public decimal Pipeline { get; set; }
    public decimal Weighted { get; set; }
}

#endregion
