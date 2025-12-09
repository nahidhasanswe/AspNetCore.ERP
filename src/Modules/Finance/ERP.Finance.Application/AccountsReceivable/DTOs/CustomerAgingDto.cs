namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerAgingDto(
    Guid CustomerId,
    string CustomerName,
    decimal Current,    // Not yet due
    decimal Days1_30,   // 1-30 days overdue
    decimal Days31_60,  // 31-60 days overdue
    decimal Days61_90,  // 61-90 days overdue
    decimal Over90Days) // Over 90 days overdue
{
    public decimal TotalDue => Current + Days1_30 + Days31_60 + Days61_90 + Over90Days;
}