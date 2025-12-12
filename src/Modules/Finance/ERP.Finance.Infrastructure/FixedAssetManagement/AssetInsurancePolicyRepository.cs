using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.FixedAssetManagement;

public class AssetInsurancePolicyRepository : EfRepository<FinanceDbContext, AssetInsurancePolicy>, IAssetInsurancePolicyRepository
{
    public AssetInsurancePolicyRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}