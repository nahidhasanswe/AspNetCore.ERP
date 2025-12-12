using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.FixedAssetManagement;

public class FixedAssetRepository : EfRepository<FinanceDbContext, FixedAsset>, IFixedAssetRepository
{
    public FixedAssetRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<IReadOnlyCollection<FixedAsset>> GetAllActiveAssetsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}