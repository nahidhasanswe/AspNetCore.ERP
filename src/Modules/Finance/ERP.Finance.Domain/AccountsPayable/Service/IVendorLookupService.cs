namespace ERP.Finance.Domain.AccountsPayable.Service;

public interface IVendorLookupService
{
    Task<string> GetVendorNameAsync(Guid vendorId);
}