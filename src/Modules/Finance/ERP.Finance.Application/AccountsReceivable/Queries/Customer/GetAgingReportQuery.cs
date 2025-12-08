using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsReceivable.DTOs;

namespace ERP.Finance.Application.AccountsReceivable.Queries.Customer;

public class GetAgingReportQuery : IQuery<Result<List<CustomerAgingDto>>>
{
    public DateTime AsOfDate { get; set; }
}