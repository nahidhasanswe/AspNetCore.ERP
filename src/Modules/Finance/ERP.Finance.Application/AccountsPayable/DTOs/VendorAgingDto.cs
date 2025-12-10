namespace ERP.Finance.Application.AccountsPayable.DTOs;

public class VendorAgingDto
{
    public Guid VendorId { get; set; }
    public string VendorName { get; set; }
    public decimal Current { get; set; }    // Not yet due
    public decimal Days1_30 { get; set; }   // 1-30 days overdue
    public decimal Days31_60 { get; set; }
    public decimal Days61_90 { get; set; }
    public decimal Over90Days { get; set; }
    public decimal TotalDue => Current + Days1_30 + Days31_60 + Days61_90 + Over90Days;
}