using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.TaxManagement;

public class TaxJurisdictionRepository : EfRepository<FinanceDbContext, TaxJurisdiction>, ITaxJurisdictionRepository
{
    public TaxJurisdictionRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}