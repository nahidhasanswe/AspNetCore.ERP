using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorPaymentHistory;

public class GetVendorPaymentHistoryQueryHandler(IVendorRepository repository)
    : IRequestCommandHandler<GetVendorPaymentHistoryQuery, List<VendorPaymentDto>>
{
    public async Task<Result<List<VendorPaymentDto>>> Handle(GetVendorPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPaymentHistoryAsync(request.VendorId, request.StartDate, request.EndDate, cancellationToken);
        return Result.Success(result);
    }
}