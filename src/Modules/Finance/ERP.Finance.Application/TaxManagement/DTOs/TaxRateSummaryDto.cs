using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxRateSummaryDto(
    Guid Id,
    Guid JurisdictionId,
    string JurisdictionName,
    decimal Rate,
    DateTime EffectiveDate,
    bool IsActive
);