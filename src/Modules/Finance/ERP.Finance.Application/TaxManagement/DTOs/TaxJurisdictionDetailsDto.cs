using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxJurisdictionDetailsDto(
    Guid Id,
    string Name,
    string RegionCode,
    bool IsActive
);