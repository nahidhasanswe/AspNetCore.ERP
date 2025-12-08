using ERP.Core;
using ERP.Finance.Domain.GeneralLedger.Enums;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateAccount;

public class CreateAccountCommand : IRequest<Result<Guid>>
{
    public string AccountCode { get; set; }
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; } // For creating hierarchies
    public bool IsSummary { get; set; } // To flag as a non-postable summary account
}