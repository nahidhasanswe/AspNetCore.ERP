using ERP.Core;
using ERP.Finance.Application.FiscalYear.DTOs;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Queries.GetFiscalPeriodById;

public class GetFiscalPeriodByIdQueryHandler(IFiscalPeriodRepository fiscalPeriodRepository)
    : IRequestHandler<GetFiscalPeriodByIdQuery, Result<FiscalPeriodDto>>
{
    public async Task<Result<FiscalPeriodDto>> Handle(GetFiscalPeriodByIdQuery request, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetByIdAsync(request.FiscalPeriodId, cancellationToken);
        if (fiscalPeriod == null)
        {
            return Result.Failure<FiscalPeriodDto>("Fiscal Period not found.");
        }

        var dto = new FiscalPeriodDto(
            fiscalPeriod.Id,
            fiscalPeriod.Name,
            fiscalPeriod.StartDate,
            fiscalPeriod.EndDate,
            fiscalPeriod.Status.ToString()
        );

        return Result.Success(dto);
    }
}