using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Queries.ListEncumbrances;

public class ListEncumbrancesQueryHandler(
    IEncumbranceRepository encumbranceRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<ListEncumbrancesQuery, Result<IEnumerable<EncumbranceSummaryDto>>>
{
    public async Task<Result<IEnumerable<EncumbranceSummaryDto>>> Handle(ListEncumbrancesQuery request, CancellationToken cancellationToken)
    {
        var allEncumbrances = await encumbranceRepository.ListAllAsync(cancellationToken);

        var filteredEncumbrances = allEncumbrances.AsQueryable();

        if (request.SourceTransactionId.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.SourceTransactionId == request.SourceTransactionId.Value);
        }

        if (request.GlAccountId.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.GlAccountId == request.GlAccountId.Value);
        }

        if (request.Type.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.Type == request.Type.Value);
        }

        if (request.Status.HasValue)
        {
            filteredEncumbrances = filteredEncumbrances.Where(e => e.Status == request.Status.Value);
        }

        var summaryResult = new List<EncumbranceSummaryDto>();
        foreach (var enc in filteredEncumbrances)
        {
            var glAccountName = await glAccountRepository.GetAccountNameAsync(enc.GlAccountId, cancellationToken);
            summaryResult.Add(new EncumbranceSummaryDto(
                enc.Id,
                enc.SourceTransactionId,
                enc.Amount,
                enc.GlAccountId,
                glAccountName ?? "Unknown GL Account",
                enc.Type,
                enc.Status,
                enc.CreatedAt
            ));
        }

        return Result.Success(summaryResult.AsEnumerable());
    }
}