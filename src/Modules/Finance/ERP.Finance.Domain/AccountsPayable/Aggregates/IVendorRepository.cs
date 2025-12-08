using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorRepository : IRepository<Vendor>
{
    Task<Vendor?> GetByTaxIdAsync(string taxId);
    Task<string> GetNameByIdAsync(Guid vendorId);
    Task<Vendor?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Vendor?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken);
    Task<IEnumerable<Vendor>> ListAllAsync();
}