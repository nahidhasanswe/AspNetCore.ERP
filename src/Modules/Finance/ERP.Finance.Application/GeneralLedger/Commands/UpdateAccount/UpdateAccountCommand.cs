using ERP.Core;
using ERP.Finance.Domain.GeneralLedger.Enums;
using MediatR;
using System;

namespace ERP.Finance.Application.GeneralLedger.Commands.UpdateAccount;

public class UpdateAccountCommand : IRequest<Result>
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; }
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsSummary { get; set; }
    public Guid BusinessUnitId { get; set; } // Include BusinessUnitId for update
}