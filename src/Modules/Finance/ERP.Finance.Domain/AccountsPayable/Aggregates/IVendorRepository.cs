using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorRepository : IRepository<Vendor>
{
    Task<Vendor?> GetByTaxIdAsync(string taxId);
    Task<string> GetNameByIdAsync(Guid vendorId);
    Task<IEnumerable<Vendor>> ListAllAsync();
}