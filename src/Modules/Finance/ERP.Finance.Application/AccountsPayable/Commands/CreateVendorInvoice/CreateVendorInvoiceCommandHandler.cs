using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Application.TaxManagement.Commands.CalculateTax;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendorInvoice;

public class CreateVendorInvoiceCommandHandler : IRequestCommandHandler<CreateVendorInvoiceCommand, Guid>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateVendorInvoiceCommandHandler(
        IVendorInvoiceRepository invoiceRepository,
        IVendorRepository vendorRepository,
        IMediator mediator,
        IUnitOfWorkManager unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _vendorRepository = vendorRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetByIdAsync(command.VendorId, cancellationToken);
        if (vendor == null) throw new DomainException("Vendor not found.");

        var dueDate = DateTime.UtcNow.AddDays(30);
        
        //Guid apControlAccountId = await glConfigService.GetAPControlAccountAsync(command.BusinessUnitId);

        // Config driven value
        var apControlAccountId = Guid.NewGuid(); // This should ideally be fetched based on BusinessUnitId

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
        
        // 2. Integration: Calculate and record tax liability (TM module)
        // This command dispatches an event that GL will subscribe to.
        await _mediator.Send(new CalculateAndRecordTaxCommand
        {
            BaseAmount = invoice.TotalAmount, // Total amount includes tax in this example
            JurisdictionCode = command.JurisdictionCode,
            TransactionDate = command.InvoiceDate,
            SourceTransactionId = invoice.Id,
            IsSalesTransaction = false, // Indicates Purchase/AP Tax
            CostCenterId = command.CostCenterId,
            Reference = invoice.InvoiceNumber
        }, cancellationToken);

        using var scope = _unitOfWork.Begin();
        
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Dispatches InvoiceReceivedEvent (to GL)

        return Result.Success(invoice.Id);
    }
}