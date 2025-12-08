using ERP.Core;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateAccount;

public class CreateAccountCommand: IRequest<Result<Guid>>
{
    public string AccountCode { get; set; } 
    public string Name { get; set; }
    public AccountType Type { get; set; } 
    public bool IsActive { get; set; }
}