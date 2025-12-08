using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.TaxManagement.DTOs;
using ERP.Finance.Domain.TaxManagement.Events;
using ERP.Finance.Domain.TaxManagement.Service;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.CalculateTax;

public class CalculateAndRecordTaxCommandHandler(
    ITaxCalculationService taxService,
    IMediator mediator
    ) : IRequestCommandHandler<CalculateAndRecordTaxCommand, bool>
{
    public async Task<Result<bool>> Handle(CalculateAndRecordTaxCommand command, CancellationToken cancellationToken)
    {
        var request = new TaxCalculationRequest 
        { 
            BaseAmount = command.BaseAmount, 
            JurisdictionCode = command.JurisdictionCode,
            TransactionDate = command.TransactionDate,
            SourceTransactionId = command.SourceTransactionId,
            IsSalesTransaction = command.IsSalesTransaction
        };

        // 1. Execute Domain Logic
        var result = await taxService.CalculateTax(request);

        if (result.TaxAmount.Amount > 0)
        {
            // 2. Dispatch event to GL to record the liability
            var taxEvent = new TaxCalculatedEvent(
                result.SourceTransactionId,
                result.IsSalesTransaction,
                result.TaxAmount,
                result.TaxPayableAccountId,
                command.SourceControlAccountId,
                command.SourceTransactionId.ToString(), // Reference
                command.CostCenterId
            );
            
            // Dispatch immediately (MediatR INotification)
            await mediator.Publish(taxEvent, cancellationToken);
        }
        
        return Result.Success(true);
    }
}