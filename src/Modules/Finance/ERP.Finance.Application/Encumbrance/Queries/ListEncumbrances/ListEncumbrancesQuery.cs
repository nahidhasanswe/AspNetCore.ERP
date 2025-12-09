using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using ERP.Finance.Domain.Encumbrance.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.Encumbrance.Queries.ListEncumbrances;

public class ListEncumbrancesQuery : IRequest<Result<IEnumerable<EncumbranceSummaryDto>>>
{
    public Guid? SourceTransactionId { get; set; }
    public Guid? GlAccountId { get; set; }
    public EncumbranceType? Type { get; set; }
    public EncumbranceStatus? Status { get; set; }
}