using ERP.Core.Repository;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorRepository : IRepository<Vendor>
{
    Task<Vendor?> GetByTaxIdAsync(string taxId);
    Task<string> GetNameByIdAsync(Guid vendorId);
    Task<Vendor?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Vendor?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken);
    Task<IEnumerable<Vendor>> ListAllAsync();

    Task<List<VendorPaymentDto>> GetPaymentHistoryAsync(
        Guid vendorId,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default);
}