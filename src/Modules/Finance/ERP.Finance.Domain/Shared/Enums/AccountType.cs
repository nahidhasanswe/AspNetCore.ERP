using System.ComponentModel;

namespace ERP.Finance.Domain.Shared.Enums;

public enum AccountType
{   
    [Description("ASSET")]
    Asset, 
    [Description("EXPENSE")]
    Expense, 
    [Description("LIABILITY")]
    Liability
}