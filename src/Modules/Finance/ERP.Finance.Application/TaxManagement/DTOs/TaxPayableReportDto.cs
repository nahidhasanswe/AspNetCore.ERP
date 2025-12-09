using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.TaxManagement.DTOs;

public record TaxPayableReportDto(
    Guid TaxPayableAccountId,
    string TaxPayableAccountName,
    Money TotalTaxPayableAmount,
    string Currency,
    DateTime AsOfDate
);