using ERP.Core;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorPaymentHistory;

public class GetVendorPaymentHistoryQueryHandler(IVendorPaymentReadRepository readRepository)
    : IRequestCommandHandler<GetVendorPaymentHistoryQuery, List<VendorPaymentDto>>
{
    public async Task<Result<List<VendorPaymentDto>>> Handle(GetVendorPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var paymentHistory = await readRepository.GetPaymentHistoryAsync(
            request.VendorId,
            request.StartDate,
            request.EndDate
        );

        return Result.Success(paymentHistory);
    }
}