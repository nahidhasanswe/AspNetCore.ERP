using ERP.Core.Repository;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public interface IFixedAssetRepository : IRepository<FixedAsset>
{
    // Add any specific query methods needed for Fixed Assets in the future
    Task<IReadOnlyCollection<FixedAsset>> GetAllActiveAssetsAsync(CancellationToken cancellationToken = default);
}