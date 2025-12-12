using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Service;

namespace ERP.Finance.Application.AccountsPayable.Services;

public class VendorLookupService(IVendorRepository repository) : IVendorLookupService
{
    public Task<string> GetVendorNameAsync(Guid vendorId) 
    {
        return repository.GetNameByIdAsync(vendorId);
    }

    public Task<Dictionary<Guid, string>> GetVendorNamesAsync(IEnumerable<Guid> vendorIds)
    {
        throw new NotImplementedException();
    }
}