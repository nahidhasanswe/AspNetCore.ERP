using ERP.Core.Repository;
using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorInvoiceRepository : IRepository<VendorInvoice>
{
    Task<IEnumerable<VendorInvoice>> GetInvoicesDueForPayment(DateTime dueDate);
    Task<IEnumerable<VendorInvoice>> ListAllUnpaidAsync();
    Task<IEnumerable<VendorInvoice>> GetForecastProjectionAsync(DateTime dueDateCutoff);
    Task<IEnumerable<VendorInvoice>> ListInvoicesByStatusAsync(InvoiceStatus status);
}