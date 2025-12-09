using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxJurisdictionById;

public class GetTaxJurisdictionByIdQueryHandler : IRequestHandler<GetTaxJurisdictionByIdQuery, Result<TaxJurisdictionDetailsDto>>
{
    private readonly ITaxJurisdictionRepository _jurisdictionRepository;

    public GetTaxJurisdictionByIdQueryHandler(ITaxJurisdictionRepository jurisdictionRepository)
    {
        _jurisdictionRepository = jurisdictionRepository;
    }

    public async Task<Result<TaxJurisdictionDetailsDto>> Handle(GetTaxJurisdictionByIdQuery request, CancellationToken cancellationToken)
    {
        var jurisdiction = await _jurisdictionRepository.GetByIdAsync(request.JurisdictionId, cancellationToken);
        if (jurisdiction == null)
        {
            return Result.Failure<TaxJurisdictionDetailsDto>("Tax Jurisdiction not found.");
        }

        var dto = new TaxJurisdictionDetailsDto(
            jurisdiction.Id,
            jurisdiction.Name,
            jurisdiction.RegionCode,
            jurisdiction.IsActive
        );

        return Result.Success(dto);
    }
}