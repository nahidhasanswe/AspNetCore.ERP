using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetCashRequirements;

public class GetCashRequirementsQuery : IRequest<Result<IEnumerable<CashRequirementDto>>>
{
    public DateTime DueDateCutoff { get; set; }
}