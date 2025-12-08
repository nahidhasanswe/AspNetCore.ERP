namespace ERP.Finance.Application.GeneralLedger.DTOs;

public class IncomeStatementDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<IncomeStatementLine> Revenue { get; set; } = new();
    public List<IncomeStatementLine> Expenses { get; set; } = new();
    public decimal TotalRevenue => Revenue.Sum(r => r.Amount);
    public decimal TotalExpenses => Expenses.Sum(e => e.Amount);
    public decimal NetIncome => TotalRevenue - TotalExpenses;
}

public class IncomeStatementLine
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
    public int Level { get; set; } // For hierarchical display
}