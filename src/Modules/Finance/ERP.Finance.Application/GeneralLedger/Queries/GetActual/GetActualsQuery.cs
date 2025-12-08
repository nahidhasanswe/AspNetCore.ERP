using ERP.Core;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetActual;


public class GetActualsQuery : IQuery<Result<Dictionary<Guid, decimal>>>
{
    public IEnumerable<Guid> AccountIds { get; set; } = new List<Guid>();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }


    public GetActualsQuery(IEnumerable<Guid> accountIds, DateTime startDate, DateTime endDate)
    {
        AccountIds = accountIds;
        StartDate = startDate;
        EndDate = endDate;
    }
}