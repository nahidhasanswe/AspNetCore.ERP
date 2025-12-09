using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Queries.GetEncumbranceById;

public class GetEncumbranceByIdQueryHandler(
    IEncumbranceRepository encumbranceRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetEncumbranceByIdQuery, Result<EncumbranceDetailsDto>>
{
    public async Task<Result<EncumbranceDetailsDto>> Handle(GetEncumbranceByIdQuery request, CancellationToken cancellationToken)
    {
        var encumbrance = await encumbranceRepository.GetByIdAsync(request.EncumbranceId, cancellationToken);
        if (encumbrance == null)
        {
            return Result.Failure<EncumbranceDetailsDto>("Encumbrance not found.");
        }

        var glAccountName = await glAccountRepository.GetAccountNameAsync(encumbrance.GlAccountId, cancellationToken);

        var dto = new EncumbranceDetailsDto(
            encumbrance.Id,
            encumbrance.SourceTransactionId,
            encumbrance.Amount,
            encumbrance.GlAccountId,
            glAccountName ?? "Unknown GL Account",
            encumbrance.CostCenterId,
            encumbrance.Type,
            encumbrance.Status,
            encumbrance.CreatedAt
        );

        return Result.Success(dto);
    }
}