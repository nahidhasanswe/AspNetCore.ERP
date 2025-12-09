using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using MediatR;
namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCashReceipts;

public class ListCashReceiptsQuery : IRequest<Result<IEnumerable<CashReceiptSummaryDto>>>
{
    public Guid? CustomerId { get; set; }
    public ReceiptStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}