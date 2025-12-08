namespace ERP.Finance.Application.FixedAssetManagement.DTOs;

public record AssetDetailsDto(
    Guid Id,
    string TagNumber,
    decimal AcquisitionCost,
    decimal CurrentBookValue, // Calculated: Cost - Accumulated Depreciation
    decimal TotalAccumulatedDepreciation,
    bool IsFullyDepreciated
);
