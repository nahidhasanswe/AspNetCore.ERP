using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ERP.Finance.Application.FixedAssetManagement.Services;

public class DepreciationExecutionService(
        IFixedAssetRepository repository,
        IUnitOfWorkManager unitOfWork ,
        ILogger<DepreciationExecutionService> logger
    )
{
    /// <summary>
    /// Executes the depreciation calculation for all active assets for a given period.
    /// </summary>
    /// <param name="period">The month/year for which depreciation should be calculated (e.g., 2025-12-01).</param>
    /// <param name="cancellationToken"></param>
    public async Task RunMonthlyDepreciation(DateTime period, CancellationToken cancellationToken = default)
    {
        // 1. Determine the processing date range
        var processingDate = new DateTime(period.Year, period.Month, 1);
        
        // In a complex system, you would check a log to prevent running twice for the same period.
        // if (await _depreciationLog.IsProcessed(processingDate)) return;

        // 2. Load Aggregates (Domain Models)
        // This method should only fetch assets that are active and not yet fully depreciated.
        IEnumerable<FixedAsset> activeAssets = await repository.GetAllActiveAssetsAsync(cancellationToken);
        
        if (!activeAssets.Any())
        {
            logger.LogWarning("No active assets found for depreciation run");
            return;
        }

        using var scope = unitOfWork.Begin();

        // 3. Execute Domain Logic on each Aggregate
        int assetsProcessed = 0;
        int entriesCreated = 0;
        
        foreach (var asset in activeAssets)
        {
            // Check if the asset was acquired in the current month (often requires special handling, 
            // but for simplicity, we rely on the domain invariant inside Depreciate).
            if (asset.AcquisitionDate.Date > processingDate.Date) continue;

            try
            {
                // The Depreciate method is the core business rule. 
                // It calculates the amount, updates the asset's internal state, 
                // and raises the DepreciationPostedEvent.
                asset.Depreciate(processingDate, out Money amountDepreciated);

                if (amountDepreciated.Amount > 0)
                {
                    entriesCreated++;
                }
                assetsProcessed++;

                await repository.UpdateAsync(asset, cancellationToken);
            }
            catch (DomainException ex)
            {
                logger.LogError(ex, $"Error processing asset {asset.Id}: {ex.Message}");
            }
        }
        
        // 4. Persistence and Event Dispatch
        // The SaveChangesAsync call, when configured with MediatR/Unit of Work, 
        // commits the database changes AND dispatches all accumulated Domain Events 
        // (DepreciationPostedEvent) to the GL Handler.
        await scope.SaveChangesAsync(cancellationToken); 
        
        logger.LogInformation($"Successfully processed {assetsProcessed} assets and created {entriesCreated} GL events.");
    }
}