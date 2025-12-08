using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.AccountPayable;

public class GetAccountsPayableAgingQuery : IResultQuery<List<VendorAgingDto>>
{
    public DateTime AsOfDate { get; set; }
}