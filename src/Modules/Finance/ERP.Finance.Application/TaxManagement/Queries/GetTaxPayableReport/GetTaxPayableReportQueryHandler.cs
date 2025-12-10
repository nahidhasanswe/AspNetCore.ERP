using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxPayableReport;

public class GetTaxPayableReportQueryHandler(
    ITaxJurisdictionRepository jurisdictionRepository,
    IAccountRepository accountRepository,
    IGLReportingService glReportingService,
    IGLConfigurationService glConfigurationService)
    : IRequestHandler<GetTaxPayableReportQuery, Result<IEnumerable<TaxPayableReportDto>>>
{
    public async Task<Result<IEnumerable<TaxPayableReportDto>>> Handle(GetTaxPayableReportQuery request, CancellationToken cancellationToken)
    {
        var reportDtos = new List<TaxPayableReportDto>();

        // Get all active tax jurisdictions
        var activeJurisdictions = (await jurisdictionRepository.ListAllAsync(cancellationToken))
            .Where(j => j.IsActive)
            .ToList();

        foreach (var jurisdiction in activeJurisdictions)
        {
            // Dynamically resolve the TaxPayableAccountId for this jurisdiction and business unit
            var taxPayableAccountId = await glConfigurationService.GetTaxPayableAccountId(jurisdiction.Id, request.BusinessUnitId);
            
            if (taxPayableAccountId != Guid.Empty) // Only proceed if an account is configured
            {
                var accountName = await accountRepository.GetAccountNameAsync(taxPayableAccountId, cancellationToken);
                var balance = await glReportingService.GetAccountBalanceAsOfDate(taxPayableAccountId, request.AsOfDate);

                if (balance.Amount != 0) // Only show accounts with an outstanding balance
                {
                    reportDtos.Add(new TaxPayableReportDto(
                        taxPayableAccountId,
                        accountName ?? "Unknown Account",
                        balance,
                        balance.Currency,
                        request.AsOfDate
                    ));
                }
            }
        }

        return Result.Success(reportDtos.AsEnumerable());
    }
}