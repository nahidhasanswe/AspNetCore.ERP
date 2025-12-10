using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxRateDetailsDto(
    Guid Id,
    Guid JurisdictionId,
    string JurisdictionName, // Populated by handler
    decimal Rate,
    DateTime EffectiveDate,
    bool IsActive
);