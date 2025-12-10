using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomerInvoice;

public class CreateCustomerInvoiceCommandHandler(
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateCustomerInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCustomerInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var lineItems = command.LineItems.Select(dto => new CustomerInvoiceLineItem(
            dto.Description,
            dto.LineAmount,
            dto.RevenueAccountId,
            dto.CostCenterId
        )).ToList();

        var invoice = CustomerInvoice.CreateDraft(
            command.BusinessUnitId,
            command.CustomerId,
            command.InvoiceNumber,
            command.ARControlAccountId,
            command.DueDate,
            command.CostCenterId,
            lineItems
        );

        await invoiceRepository.AddAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}