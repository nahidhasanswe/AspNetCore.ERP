using ERP.Core.Specifications;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Shared.Enums;
using LinqKit;

namespace ERP.Finance.Domain.Budgeting.Specifications;

public class GetBudgetSpecifications : Specification<Budget>
{
    public GetBudgetSpecifications(Guid? businessUnitId, string? fiscalPeriod, BudgetStatus? status)
    {
        AddInclude(x => x.Items);
        
        var query = PredicateBuilder.New<Budget>(true);

        if (businessUnitId.HasValue)
            query.And(x => x.BusinessUnitId == businessUnitId);

        if (!string.IsNullOrWhiteSpace(fiscalPeriod))
            query.And(x => x.FiscalPeriod == fiscalPeriod);
        
        if (status.HasValue)
            query.And(x => x.Status == status);
        
        AddCriteria(query);
    }
}