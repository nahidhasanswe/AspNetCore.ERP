using ERP.Core;
using ERP.Core.Entities;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class Vendor : Entity
{
    public string Name { get; private set; }
    public string TaxId { get; private set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string PaymentTerms { get; private set; } // e.g., NET 30, NET 60
    
    private Vendor() { }

    public Vendor(string name, string taxId, string email, string phone, string paymentTerms) : base(Guid.NewGuid())
    {
        Name = name;
        TaxId = taxId;
        PaymentTerms = paymentTerms;
        ContactEmail = email;
        ContactPhone = phone;
    }


    public Result UpdateInfo(string name, string taxId, string email, string phone, string paymentTerms)
    {
        Name = name;
        TaxId = taxId;
        PaymentTerms = paymentTerms;
        ContactEmail = email;
        ContactPhone = phone;
        
        return Result.Success();
    }
}