using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.GeneralLedger.DTOs;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetTrialBalance;

// We can add more parameters later, like AsOfDate, PeriodId, etc.
public class GetTrialBalanceQuery : IRequestCommand<TrialBalanceDto>
{
}