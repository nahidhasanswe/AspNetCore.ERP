using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorAging;

public class GetVendorAgingQuery : IRequestCommand<IEnumerable<VendorAgingDto>>
{
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}