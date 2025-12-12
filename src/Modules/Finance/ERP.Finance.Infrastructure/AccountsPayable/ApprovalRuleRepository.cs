using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class ApprovalRuleRepository(IDbContextProvider<FinanceDbContext> dbContextProvider)
    : EfRepository<FinanceDbContext, ApprovalRule>(dbContextProvider), IApprovalRuleRepository
{
    public async Task<IEnumerable<ApprovalRule>> GetApplicableRulesAsync(Guid? businessUnitId, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();

        if (businessUnitId.HasValue)
            query = query.Where(x => x.BusinessUnitId == businessUnitId);

        return await query.ToListAsync(cancellationToken);
    }
}