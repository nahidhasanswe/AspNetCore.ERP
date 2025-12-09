using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomerInvoice;

public class CreateCustomerInvoiceCommandHandler : IRequestCommandHandler<CreateCustomerInvoiceCommand, Guid>
{
    private readonly ICustomerInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateCustomerInvoiceCommandHandler(ICustomerInvoiceRepository invoiceRepository, IUnitOfWorkManager unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var lineItems = command.LineItems.Select(dto => new CustomerInvoiceLineItem(
            dto.Description,
            dto.LineAmount,
            dto.RevenueAccountId,
            dto.CostCenterId
        )).ToList();

        var invoice = CustomerInvoice.CreateDraft(
            command.CustomerId,
            command.InvoiceNumber,
            command.ARControlAccountId,
            command.DueDate,
            command.CostCenterId,
            lineItems
        );

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}