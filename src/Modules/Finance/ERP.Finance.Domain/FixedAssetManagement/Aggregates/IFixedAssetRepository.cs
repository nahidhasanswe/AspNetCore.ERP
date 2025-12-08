using ERP.Core.Repository;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public interface IFixedAssetRepository : IRepository<FixedAsset>
{
    Task<IEnumerable<FixedAsset>> GetAllActiveAssetsAsync(CancellationToken cancellationToken);
}