namespace ERP.Finance.Domain.FiscalYear.Enums;

public enum PeriodStatus
{
    Open,           // Transactions allowed
    SoftClose,      // Only adjustments/admin transactions allowed
    HardClose       // No transactions allowed
}