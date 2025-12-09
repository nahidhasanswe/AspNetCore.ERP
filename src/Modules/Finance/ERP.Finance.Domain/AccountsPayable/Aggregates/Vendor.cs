using ERP.Core;
using ERP.Core.Entities;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class Vendor : Entity
{
    public string Name { get; private set; }
    public string TaxId { get; private set; }
    public Address Address { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public string PaymentTerms { get; private set; }
    public string DefaultCurrency { get; private set; }
    public VendorBankDetails VendorBankDetails { get; private set; }
    public bool IsActive { get; private set; }

    private Vendor() { }

    public Vendor(string name, string taxId, Address address, ContactInfo contactInfo, string paymentTerms, string defaultCurrency, VendorBankDetails bankDetails) : base(Guid.NewGuid())
    {
        Name = name;
        TaxId = taxId;
        Address = address;
        ContactInfo = contactInfo;
        PaymentTerms = paymentTerms;
        DefaultCurrency = defaultCurrency;
        VendorBankDetails = bankDetails;
        IsActive = true;
    }

    public Result UpdateInfo(string name, string taxId, Address address, ContactInfo contactInfo, string paymentTerms, string defaultCurrency, VendorBankDetails bankDetails, bool isActive)
    {
        Name = name;
        TaxId = taxId;
        Address = address;
        ContactInfo = contactInfo;
        PaymentTerms = paymentTerms;
        DefaultCurrency = defaultCurrency;
        VendorBankDetails = bankDetails;
        IsActive = isActive; // Update IsActive status

        return Result.Success();
    }

    public Result Activate()
    {
        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        IsActive = false;
        return Result.Success();
    }
}