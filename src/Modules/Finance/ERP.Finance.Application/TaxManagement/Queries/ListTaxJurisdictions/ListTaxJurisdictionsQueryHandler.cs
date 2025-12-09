using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Queries.ListTaxJurisdictions;

public class ListTaxJurisdictionsQueryHandler(ITaxJurisdictionRepository jurisdictionRepository)
    : IRequestHandler<ListTaxJurisdictionsQuery, Result<IEnumerable<TaxJurisdictionSummaryDto>>>
{
    public async Task<Result<IEnumerable<TaxJurisdictionSummaryDto>>> Handle(ListTaxJurisdictionsQuery request, CancellationToken cancellationToken)
    {
        var allJurisdictions = await jurisdictionRepository.ListAllAsync(cancellationToken);

        var filteredJurisdictions = allJurisdictions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredJurisdictions = filteredJurisdictions.Where(j => j.Name.Contains(request.Name));
        }

        if (request.IsActive.HasValue)
        {
            filteredJurisdictions = filteredJurisdictions.Where(j => j.IsActive == request.IsActive.Value);
        }

        var summaryDtos = filteredJurisdictions.Select(j => new TaxJurisdictionSummaryDto(
            j.Id,
            j.Name,
            j.RegionCode,
            j.IsActive
        )).ToList();

        return Result.Success(summaryDtos.AsEnumerable());
    }
}