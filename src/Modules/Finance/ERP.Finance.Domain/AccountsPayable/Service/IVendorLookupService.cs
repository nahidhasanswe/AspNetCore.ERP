namespace ERP.Finance.Domain.AccountsPayable.Service;

public interface IVendorLookupService
{
    Task<string> GetVendorNameAsync(Guid vendorId);
    Task<Dictionary<Guid, string>> GetVendorNamesAsync(IEnumerable<Guid> vendorIds); // New method
}