using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Infrastructure.Database;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class CreditMemoRepository(IDbContextProvider<FinanceDbContext> dbContextProvider)
    : EfRepository<FinanceDbContext, CreditMemo>(dbContextProvider), ICreditMemoRepository;