using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Application.TaxManagement.Commands.CalculateTax;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendorInvoice;

public class CreateVendorInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IVendorRepository vendorRepository,
    IGLConfigurationService glConfigService,
    IMediator mediator,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateVendorInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var vendor = await vendorRepository.GetByIdAsync(command.VendorId, cancellationToken);
        if (vendor == null) throw new DomainException("Vendor not found.");

        var dueDate = DateTime.UtcNow.AddDays(30);
        
        Guid apControlAccountId = await glConfigService.GetAPControlAccountAsync(command.BusinessUnitId, command.Currency, cancellationToken);
        
        // 1. Create the Aggregate Root
        var invoice = VendorInvoice.CreateNonPOInvoice(
            command.BusinessUnitId, // Pass BusinessUnitId
            command.VendorId, 
            command.InvoiceNumber, 
            command.InvoiceDate,
            dueDate,
            apControlAccountId,
            command.CostCenterId,
            command.InvoiceLines.Select(l => new InvoiceLineItem(l.Description, l.LineAmount, l.ExpenseAccountId, l.CostCenterId))
        );
        
        // Integration: Calculate and record tax liability (TM module)
        // This command dispatches an event that GL will subscribe to.
        await mediator.Send(new CalculateAndRecordTaxCommand
        {
            BaseAmount = invoice.TotalAmount, // Total amount includes tax in this example
            JurisdictionCode = command.JurisdictionCode,
            TransactionDate = command.InvoiceDate,
            SourceTransactionId = invoice.Id,
            IsSalesTransaction = false, // Indicates Purchase/AP Tax
            CostCenterId = command.CostCenterId,
            Reference = invoice.InvoiceNumber,
            BusinessUnitId = command.BusinessUnitId // Pass BusinessUnitId to tax calculation
        }, cancellationToken);

        using var scope = unitOfWork.Begin();
        
        await invoiceRepository.AddAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Dispatches InvoiceReceivedEvent (to GL)

        return Result.Success(invoice.Id);
    }
}