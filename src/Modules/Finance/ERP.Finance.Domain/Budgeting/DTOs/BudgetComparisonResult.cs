namespace ERP.Finance.Domain.Budgeting.DTOs;

public class BudgetComparisonResult
{
    public string AccountName { get; set; }
    public string Period { get; set; }
    public decimal Budgeted { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance => Budgeted - Actual;
}