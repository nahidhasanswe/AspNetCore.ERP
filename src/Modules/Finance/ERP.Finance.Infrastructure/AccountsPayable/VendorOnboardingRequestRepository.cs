using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class VendorOnboardingRequestRepository (IDbContextProvider<FinanceDbContext> dbContextProvider) : EfRepository<FinanceDbContext, VendorOnboardingRequest>(dbContextProvider), IVendorOnboardingRequestRepository
{
    public async Task<IReadOnlyCollection<VendorOnboardingRequest>> GetAllAsync(OnboardingStatus? status, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        return await query.ToListAsync(cancellationToken);
    }
}