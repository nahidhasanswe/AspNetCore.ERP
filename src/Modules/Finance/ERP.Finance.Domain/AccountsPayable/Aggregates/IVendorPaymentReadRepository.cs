using ERP.Core.Repository;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorPaymentReadRepository : IReadRepository<VendorInvoice>
{
    /// <summary>
    /// Retrieves payment records for a specific vendor within a date range.
    /// </summary>
    Task<List<VendorPaymentDto>> GetPaymentHistoryAsync(
        Guid vendorId, 
        DateTime? startDate, 
        DateTime? endDate);
}