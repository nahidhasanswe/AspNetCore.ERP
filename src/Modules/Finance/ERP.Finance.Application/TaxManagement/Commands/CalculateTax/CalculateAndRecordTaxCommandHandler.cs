using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Domain.TaxManagement.Events;
using ERP.Finance.Domain.TaxManagement.Service;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.CalculateTax;

public class CalculateAndRecordTaxCommandHandler(
    ITaxCalculationService taxCalculationService,
    ITaxJurisdictionRepository jurisdictionRepository,
    IUnitOfWorkManager unitOfWork,
    IMediator mediator)
    : IRequestCommandHandler<CalculateAndRecordTaxCommand, bool>
{
    // To get JurisdictionId from Code
    // For saving events
    // To publish events

    public async Task<Result<bool>> Handle(CalculateAndRecordTaxCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        // 1. Get JurisdictionId from JurisdictionCode
        // This assumes a method like GetByCodeAsync exists on ITaxJurisdictionRepository
        var jurisdiction = (await jurisdictionRepository.ListAllAsync(cancellationToken))
            .FirstOrDefault(j => j.RegionCode == command.JurisdictionCode && j.IsActive);

        if (jurisdiction == null)
        {
            return Result.Failure<bool>($"Tax jurisdiction with code {command.JurisdictionCode} not found or is inactive.");
        }

        // 2. Calculate Tax
        var calculationResult = await taxCalculationService.CalculateTax(
            jurisdiction.Id,
            command.BaseAmount,
            command.TransactionDate
        );

        if (calculationResult.IsFailure)
        {
            return Result.Failure<bool>(calculationResult.Error);
        }

        var (taxAmount, taxPayableAccountId) = calculationResult.Value;

        // 3. Raise TaxCalculatedEvent for GL posting
        var taxCalculatedEvent = new TaxCalculatedEvent(
            Guid.NewGuid(), // Unique ID for this tax transaction
            command.SourceTransactionId,
            command.BaseAmount,
            taxAmount,
            jurisdiction.Id,
            command.JurisdictionCode,
            command.TransactionDate,
            taxPayableAccountId,
            command.SourceControlAccountId,
            command.IsSalesTransaction,
            command.CostCenterId,
            command.Reference
        );

        // Publish the event directly through Mediator
        await mediator.Publish(taxCalculatedEvent, cancellationToken);

        // Note: No aggregate needs to be saved here, as the event itself is the record of the calculation.
        // The GL handler will create the actual journal entry.

        await scope.SaveChangesAsync(cancellationToken); // This will ensure the event is dispatched

        return Result.Success(true);
    }
}