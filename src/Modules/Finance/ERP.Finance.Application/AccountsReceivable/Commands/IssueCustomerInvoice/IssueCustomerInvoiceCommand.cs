using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.IssueCustomerInvoice;

public class IssueCustomerInvoiceCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public DateTime IssueDate { get; set; }
}