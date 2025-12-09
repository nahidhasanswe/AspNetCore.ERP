using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateCreditMemo;

public class CreateCreditMemoCommand : IRequestCommand<Guid>
{
    public Guid VendorId { get; set; }
    public Guid BusinessUnitId { get; set; }
    public Money Amount { get; set; }
    public DateTime MemoDate { get; set; }
    public string Reason { get; set; }
}