using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Queries.ListTaxRates;

public class ListTaxRatesQueryHandler(
    ITaxRateRepository taxRateRepository,
    ITaxJurisdictionRepository jurisdictionRepository)
    : IRequestHandler<ListTaxRatesQuery, Result<IEnumerable<TaxRateSummaryDto>>>
{
    public async Task<Result<IEnumerable<TaxRateSummaryDto>>> Handle(ListTaxRatesQuery request, CancellationToken cancellationToken)
    {
        var allTaxRates = await taxRateRepository.ListAllAsync(cancellationToken);

        var filteredTaxRates = allTaxRates.AsQueryable();

        if (request.JurisdictionId.HasValue)
        {
            filteredTaxRates = filteredTaxRates.Where(tr => tr.JurisdictionId == request.JurisdictionId.Value);
        }

        if (request.AsOfDate.HasValue)
        {
            filteredTaxRates = filteredTaxRates.Where(tr => tr.EffectiveDate <= request.AsOfDate.Value);
        }

        if (request.IsActive.HasValue)
        {
            filteredTaxRates = filteredTaxRates.Where(tr => tr.IsActive == request.IsActive.Value);
        }

        var summaryDtos = new List<TaxRateSummaryDto>();
        foreach (var tr in filteredTaxRates)
        {
            var jurisdiction = await jurisdictionRepository.GetByIdAsync(tr.JurisdictionId, cancellationToken);
            summaryDtos.Add(new TaxRateSummaryDto(
                tr.Id,
                tr.JurisdictionId,
                jurisdiction?.Name ?? "Unknown Jurisdiction",
                tr.Rate,
                tr.EffectiveDate,
                tr.IsActive
            ));
        }

        return Result.Success(summaryDtos.AsEnumerable());
    }
}