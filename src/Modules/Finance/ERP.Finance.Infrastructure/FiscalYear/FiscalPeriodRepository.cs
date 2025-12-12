using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.FiscalYear;

public class FiscalPeriodRepository : EfRepository<FinanceDbContext, FiscalPeriod>, IFiscalPeriodRepository
{
    public FiscalPeriodRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public Task<FiscalPeriod?> GetPeriodByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<FiscalPeriod?> GetPeriodByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<FiscalPeriod>> ListAllAsync()
    {
        throw new NotImplementedException();
    }
}