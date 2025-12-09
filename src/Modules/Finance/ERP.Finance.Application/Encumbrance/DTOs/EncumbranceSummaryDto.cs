using ERP.Finance.Domain.Encumbrance.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.Encumbrance.DTOs;

public record EncumbranceSummaryDto(
    Guid Id,
    Guid SourceTransactionId,
    Money Amount,
    Guid GlAccountId,
    string GlAccountName,
    EncumbranceType Type,
    EncumbranceStatus Status,
    DateTime CreatedDate
);