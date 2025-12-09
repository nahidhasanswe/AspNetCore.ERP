using ERP.Finance.Domain.FixedAssetManagement.Enums;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.DTOs;

public record AssetDetailsDto(
    Guid Id,
    string TagNumber,
    string Description,
    DateTime AcquisitionDate,
    Money AcquisitionCost,
    decimal CurrentBookValue, // Calculated: Cost - Accumulated Depreciation
    decimal TotalAccumulatedDepreciation,
    bool IsFullyDepreciated,
    FixedAssetStatus Status,
    Guid AssetAccountId,
    string AssetAccountName, // Populated by handler
    Guid DepreciationExpenseAccountId,
    string DepreciationExpenseAccountName, // Populated by handler
    Guid AccumulatedDepreciationAccountId,
    string AccumulatedDepreciationAccountName, // Populated by handler
    DepreciationMethod DepreciationMethod,
    int UsefulLifeYears,
    decimal SalvageValue,
    Guid? CostCenterId
);