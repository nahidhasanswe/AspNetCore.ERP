using ERP.Core.Repository;

namespace ERP.Finance.Domain.FiscalYear.Aggregates;

public interface IFiscalPeriodRepository : IRepository<FiscalPeriod>
{
    // Critical Query for Posting Check
    Task<FiscalPeriod?> GetPeriodByDateAsync(DateTime date, CancellationToken cancellationToken);
    
    Task<FiscalPeriod?> GetPeriodByNameAsync(string name, CancellationToken cancellationToken = default);
    
    // For listing/administration
    Task<IEnumerable<FiscalPeriod>> ListAllAsync();
}