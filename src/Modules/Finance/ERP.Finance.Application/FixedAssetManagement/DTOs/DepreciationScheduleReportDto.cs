using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.DTOs;

public record DepreciationScheduleReportDto(
    Guid AssetId,
    string TagNumber,
    string Description,
    DateTime PeriodDate,
    decimal DepreciationAmount,
    decimal AccumulatedDepreciation,
    decimal BookValue
);