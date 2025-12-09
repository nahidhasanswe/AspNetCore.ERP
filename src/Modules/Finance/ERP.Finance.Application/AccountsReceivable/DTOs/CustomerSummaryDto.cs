using ERP.Finance.Domain.AccountsReceivable.Enums;
using System;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerSummaryDto(
    Guid Id,
    string Name,
    string ContactEmail,
    CustomerStatus Status
);