using System;

namespace ERP.Finance.Application.FiscalYear.DTOs;

public record FiscalPeriodDto(
    Guid Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string Status // String representation of PeriodStatus
);