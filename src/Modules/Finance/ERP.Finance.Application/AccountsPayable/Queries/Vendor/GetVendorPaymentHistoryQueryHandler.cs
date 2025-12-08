using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.Vendor;

public class GetVendorPaymentHistoryQueryHandler(
    IVendorPaymentReadRepository readRepository 
) : IRequestCommandHandler<GetVendorPaymentHistoryQuery, List<VendorPaymentDto>>
{
    public async Task<Result<List<VendorPaymentDto>>> Handle(GetVendorPaymentHistoryQuery query, CancellationToken cancellationToken)
    {
        if (query.VendorId == Guid.Empty)
        {
            throw new ArgumentException("Vendor ID is required for payment history query.");
        }
        
        // Use today's date if no end date is provided
        var endDate = query.EndDate ?? DateTime.Today;

        // 1. Fetch data from the optimized Read Model
        var history = await readRepository.GetPaymentHistoryAsync(
            query.VendorId, 
            query.StartDate, 
            endDate
        );

        // 2. Return the results (no complex business logic, just retrieval)
        return Result.Success(history);
    }
}