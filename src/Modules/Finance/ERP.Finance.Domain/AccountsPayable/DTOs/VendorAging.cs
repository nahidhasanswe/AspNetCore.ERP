namespace ERP.Finance.Domain.AccountsPayable.DTOs;

public class VendorAging {
    public Guid VendorId { get; set; }
    public string VendorName { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int DaysOverdue { get; set; }
};