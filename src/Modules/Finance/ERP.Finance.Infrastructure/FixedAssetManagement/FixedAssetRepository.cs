using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.FixedAssetManagement;

public class FixedAssetRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) : EfRepository<FinanceDbContext, FixedAsset>(dbContextProvider), IFixedAssetRepository
{
    public async Task<IReadOnlyCollection<FixedAsset>> GetAllActiveAssetsAsync(CancellationToken cancellationToken)
    {
        return await Table
            .Where(asset => 
                // 1. Must be Active (or not yet fully disposed/retired)
                // Assuming a property 'IsDisposed' or similar status check exists
                // asset.IsDisposed == false && 

                // 2. Must not be fully depreciated
                // The TotalAccumulatedDepreciation must be less than 
                // (AcquisitionCost - SalvageValue)
                asset.TotalAccumulatedDepreciation < (asset.AcquisitionCost.Amount - asset.Schedule.SalvageValue)
            )
            // Critical: Include the required dependent data for domain logic execution
            .Include(a => a.Schedule) 
            // Use AsNoTracking() if you only intend to read, modify one property, 
            // and then save, but since we modify the aggregate state, we typically track it.
            .ToListAsync(cancellationToken);
    }
}