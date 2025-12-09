using ERP.Finance.Domain.Encumbrance.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Application.Encumbrance.DTOs;

public record EncumbranceDetailsDto(
    Guid Id,
    Guid SourceTransactionId,
    Money Amount,
    Guid GlAccountId,
    string GlAccountName, // Will be populated by handler
    Guid? CostCenterId,
    EncumbranceType Type,
    EncumbranceStatus Status,
    DateTime CreatedDate
);