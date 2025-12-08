namespace ERP.Finance.Application.GeneralLedger.DTOs;

public class TrialBalanceDto
{
    public List<TrialBalanceLineDto> Lines { get; set; } = new();
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public bool IsBalanced => TotalDebits == TotalCredits;
}

public class TrialBalanceLineDto
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}