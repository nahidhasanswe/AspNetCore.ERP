using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxJurisdictionSummaryDto(
    Guid Id,
    string Name,
    string RegionCode,
    bool IsActive
);