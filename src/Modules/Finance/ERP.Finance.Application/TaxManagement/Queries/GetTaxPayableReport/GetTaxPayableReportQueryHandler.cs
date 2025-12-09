using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxPayableReport;

public class GetTaxPayableReportQueryHandler(
    ITaxRateRepository taxRateRepository,
    IAccountRepository glAccountRepository,
    IGLReportingService glReportingService)
    : IRequestHandler<GetTaxPayableReportQuery, Result<IEnumerable<TaxPayableReportDto>>>
{
    public async Task<Result<IEnumerable<TaxPayableReportDto>>> Handle(GetTaxPayableReportQuery request, CancellationToken cancellationToken)
    {
        // Get all unique TaxPayableAccountIds from active tax rates
        var taxPayableAccountIds = (await taxRateRepository.ListAllAsync(cancellationToken))
            .Where(tr => tr.IsActive)
            .Select(tr => tr.TaxPayableAccountId)
            .Distinct()
            .ToList();

        var reportDtos = new List<TaxPayableReportDto>();

        foreach (var accountId in taxPayableAccountIds)
        {
            var accountName = await glAccountRepository.GetAccountNameAsync(accountId, cancellationToken);
            var balance = await glReportingService.GetAccountBalanceAsOfDate(accountId, request.AsOfDate);

            if (balance.Amount != 0) // Only show accounts with an outstanding balance
            {
                reportDtos.Add(new TaxPayableReportDto(
                    accountId,
                    accountName ?? "Unknown Account",
                    balance,
                    balance.Currency,
                    request.AsOfDate
                ));
            }
        }

        return Result.Success(reportDtos.AsEnumerable());
    }
}