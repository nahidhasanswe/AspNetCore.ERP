using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.Vendor;

public class GetVendorPaymentHistoryQuery : IRequestCommand<List<VendorPaymentDto>>
{
    public Guid VendorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}