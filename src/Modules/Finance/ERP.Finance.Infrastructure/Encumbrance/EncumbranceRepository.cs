using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.Encumbrance;

public class EncumbranceRepository : EfRepository<FinanceDbContext, Domain.Encumbrance.Aggregates.Encumbrance>, IEncumbranceRepository
{
    public EncumbranceRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}