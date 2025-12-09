using ERP.Finance.Domain.FixedAssetManagement.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.DTOs;

public record AssetSummaryDto(
    Guid Id,
    string TagNumber,
    string Description,
    DateTime AcquisitionDate,
    Money AcquisitionCost,
    decimal TotalAccumulatedDepreciation,
    FixedAssetStatus Status
);