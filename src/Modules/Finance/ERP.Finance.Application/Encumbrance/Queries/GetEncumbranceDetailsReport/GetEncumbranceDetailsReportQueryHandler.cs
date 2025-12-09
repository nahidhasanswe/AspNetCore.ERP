using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Queries.GetEncumbranceDetailsReport;

public class GetEncumbranceDetailsReportQueryHandler(
    IEncumbranceRepository encumbranceRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetEncumbranceDetailsReportQuery, Result<IEnumerable<EncumbranceDetailsDto>>>
{
    private readonly IAccountRepository _glAccountRepository = glAccountRepository;

    public async Task<Result<IEnumerable<EncumbranceDetailsDto>>> Handle(GetEncumbranceDetailsReportQuery request, CancellationToken cancellationToken)
    {
        var allEncumbrances = await encumbranceRepository.ListAllAsync(cancellationToken);

        var filteredEncumbrances = allEncumbrances.AsQueryable();

        if (request.StartDate.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.CreatedAt >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.CreatedAt <= request.EndDate.Value);
        }

        var reportResult = new List<EncumbranceDetailsDto>();
        foreach (var enc in filteredEncumbrances)
        {
            var glAccountName = await _glAccountRepository.GetAccountNameAsync(enc.GlAccountId, cancellationToken);
            reportResult.Add(new EncumbranceDetailsDto(
                enc.Id,
                enc.SourceTransactionId,
                enc.Amount,
                enc.GlAccountId,
                glAccountName ?? "Unknown GL Account",
                enc.CostCenterId,
                enc.Type,
                enc.Status,
                enc.CreatedAt
            ));
        }

        return Result.Success(reportResult.AsEnumerable());
    }
}