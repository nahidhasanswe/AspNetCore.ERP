using ERP.Core.Repository;
using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorInvoiceRepository : IRepository<VendorInvoice>
{
    Task<IReadOnlyCollection<VendorInvoice>> ListAllUnpaidAsync();
    Task<IReadOnlyCollection<VendorInvoice>> GetForecastProjectionAsync(DateTime dueDateCutoff);
    Task<IReadOnlyCollection<VendorInvoice>> GetAllListAsync(Guid? vendorId, InvoiceStatus? status, DateTime? dueDate, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
}