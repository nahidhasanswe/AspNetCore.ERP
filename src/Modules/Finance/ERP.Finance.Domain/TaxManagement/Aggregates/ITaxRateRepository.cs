using ERP.Core.Repository;

namespace ERP.Finance.Domain.TaxManagement.Aggregates;

public interface ITaxRateRepository : IRepository<TaxRate>
{
    Task<TaxRate?> GetRateByJurisdictionAndDate(string regionCode, DateTime date);
}