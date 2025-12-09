using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateDebitMemo;

public class CreateDebitMemoCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; }
    public Guid VendorId { get; set; }
    public Money Amount { get; set; }
    public DateTime MemoDate { get; set; }
    public string Reason { get; set; }
    public Guid APControlAccountId { get; set; } // Added this
}