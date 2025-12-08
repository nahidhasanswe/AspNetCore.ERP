using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomerInvoice;

public class CreateCustomerInvoiceCommandHandler(
    ICustomerInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<CreateCustomerInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCustomerInvoiceCommand command, CancellationToken cancellationToken)
    {
        // 1. Map DTOs to Domain Value Objects
        var domainLineItems = command.LineItems.Select(dto => 
            new CustomerInvoiceLineItem(
                dto.Description, 
                dto.LineAmount, 
                dto.RevenueAccountId, 
                dto.CostCenterId
            )).ToList();

        // 2. Execute Domain Logic: Create the Aggregate Root
        var invoice = new CustomerInvoice(
            command.CustomerId, 
            command.InvoiceNumber, 
            command.ARControlAccountId, // Passed to the constructor
            domainLineItems
        );
        // The CustomerInvoice constructor calculates TotalAmount and automatically raises the CustomerInvoiceCreatedEvent.

        // 3. Persist and Dispatch Event
        using var scope = unitOfWork.Begin();
        await repository.AddAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Saves state and dispatches the event

        return Result.Success(invoice.Id);
    }
}