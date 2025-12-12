using ERP.Core;
using MediatR;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorPaymentHistory;

public class GetVendorPaymentHistoryQuery : IRequest<Result<List<VendorPaymentDto>>>
{
    public Guid VendorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}