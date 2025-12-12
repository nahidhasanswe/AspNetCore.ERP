using ERP.Core.Repository;
using ERP.Finance.Domain.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorInvoiceRepository : IRepository<VendorInvoice>
{
    Task<IReadOnlyCollection<VendorAging>> ListAllUnpaidAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CashFlowProjectionDto>> GetForecastProjectionAsync(DateTime dueDateCutoff, Guid? businessUnitId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VendorSpendAnalysisDto>> GetSpendAnalysisListAsync(Guid? vendorId, InvoiceStatus? status, DateTime? dueDate, DateTime? startDate, DateTime? endDate,
        Guid? expenseAccountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VendorInvoiceSummaryDto>> GetInvoiceSummaryAsync(Guid? vendorId, InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<List<VendorPaymentDto>> GetPaymentHistoryAsync(
        Guid vendorId, 
        DateTime? startDate, 
        DateTime? endDate,
        CancellationToken cancellationToken = default);
}