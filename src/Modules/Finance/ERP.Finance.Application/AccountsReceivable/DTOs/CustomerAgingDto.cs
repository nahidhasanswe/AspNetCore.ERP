namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record CustomerAgingDto(
    Guid CustomerId,
    string CustomerName,
    decimal Current, // 0-30 days overdue
    decimal Days31_60,
    decimal Days61_90,
    decimal Over90Days)
{
    public decimal TotalDue => Current + Days31_60 + Days61_90 + Over90Days;
}