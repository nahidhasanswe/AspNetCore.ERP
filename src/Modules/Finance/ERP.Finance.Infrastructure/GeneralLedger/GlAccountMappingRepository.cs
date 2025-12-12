using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.GeneralLedger;

public class GlAccountMappingRepository : EfRepository<FinanceDbContext, GlAccountMapping>, IGlAccountMappingRepository
{
    public GlAccountMappingRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<GlAccountMapping?> GetMapping(Guid businessUnitId, GlAccountMappingType mappingType, string currency, Guid? referenceId = null)
    {
        throw new NotImplementedException();
    }
}