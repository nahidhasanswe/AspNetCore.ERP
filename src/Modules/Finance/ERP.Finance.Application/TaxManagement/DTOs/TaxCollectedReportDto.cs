using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxCollectedReportDto(
    Guid JurisdictionId,
    string JurisdictionName,
    Guid TaxPayableAccountId,
    string TaxPayableAccountName,
    Money TotalTaxCollected,
    DateTime ReportPeriodStart,
    DateTime ReportPeriodEnd
);