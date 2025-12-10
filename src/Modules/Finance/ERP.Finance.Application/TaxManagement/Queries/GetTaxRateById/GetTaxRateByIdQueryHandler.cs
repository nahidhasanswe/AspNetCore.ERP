using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxRateById;

public class GetTaxRateByIdQueryHandler(
    ITaxRateRepository taxRateRepository,
    ITaxJurisdictionRepository jurisdictionRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetTaxRateByIdQuery, Result<TaxRateDetailsDto>>
{
    public async Task<Result<TaxRateDetailsDto>> Handle(GetTaxRateByIdQuery request, CancellationToken cancellationToken)
    {
        var taxRate = await taxRateRepository.GetByIdAsync(request.TaxRateId, cancellationToken);
        if (taxRate == null)
        {
            return Result.Failure<TaxRateDetailsDto>("Tax Rate not found.");
        }

        var jurisdiction = await jurisdictionRepository.GetByIdAsync(taxRate.JurisdictionId, cancellationToken);
        var jurisdictionName = jurisdiction?.Name ?? "Unknown Jurisdiction";
        // Removed: var taxPayableAccountName = await _glAccountRepository.GetAccountNameByIdAsync(taxRate.TaxPayableAccountId);

        var dto = new TaxRateDetailsDto(
            taxRate.Id,
            taxRate.JurisdictionId,
            jurisdictionName,
            taxRate.Rate,
            taxRate.EffectiveDate,
            // Removed: taxRate.TaxPayableAccountId,
            // Removed: taxPayableAccountName ?? "Unknown Tax Payable Account",
            taxRate.IsActive
        );

        return Result.Success(dto);
    }
}