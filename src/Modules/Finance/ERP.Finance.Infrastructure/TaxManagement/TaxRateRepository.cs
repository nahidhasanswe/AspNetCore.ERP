using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.TaxManagement;

public class TaxRateRepository : EfRepository<FinanceDbContext, TaxRate>, ITaxRateRepository
{
    public TaxRateRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<TaxRate?> GetRateByJurisdictionAndDate(string regionCode, DateTime date)
    {
        throw new NotImplementedException();
    }
}