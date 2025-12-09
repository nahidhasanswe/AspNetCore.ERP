using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.TaxManagement.Aggregates; 
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxCollectedReport;

public class GetTaxCollectedReportQueryHandler(
    ITaxJurisdictionRepository jurisdictionRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetTaxCollectedReportQuery, Result<IEnumerable<TaxCollectedReportDto>>>
{
    // In a real system, you would query a read model or a dedicated reporting database
    // that has aggregated TaxCalculatedEvents. For this example, we'll simulate
    // by assuming we can query past events or a projection of them.
    // private readonly ITaxTransactionReadModelRepository _taxTransactionReadModelRepository; // Ideal for this

    // ITaxTransactionReadModelRepository taxTransactionReadModelRepository)
    // _taxTransactionReadModelRepository = taxTransactionReadModelRepository;

    public async Task<Result<IEnumerable<TaxCollectedReportDto>>> Handle(GetTaxCollectedReportQuery request, CancellationToken cancellationToken)
    {
        // Simulate fetching aggregated tax data. In reality, this would be from a read model.
        // For now, we'll create some dummy data or assume a way to get aggregated events.
        // This is a placeholder for actual data retrieval.
        var simulatedTaxData = new List<(Guid JurisdictionId, Money TaxAmount, Guid TaxPayableAccountId, DateTime TransactionDate)>
        {
            (Guid.NewGuid(), new Money(100.00m, "USD"), Guid.NewGuid(), DateTime.UtcNow.AddDays(-10)),
            (Guid.NewGuid(), new Money(50.00m, "USD"), Guid.NewGuid(), DateTime.UtcNow.AddDays(-5)),
            (Guid.NewGuid(), new Money(75.00m, "USD"), Guid.NewGuid(), DateTime.UtcNow.AddDays(-15)),
        };

        var filteredTaxData = simulatedTaxData
            .Where(td => td.TransactionDate >= request.StartDate && td.TransactionDate <= request.EndDate)
            .ToList();

        var groupedTaxData = filteredTaxData
            .GroupBy(td => new { td.JurisdictionId, td.TaxPayableAccountId, td.TaxAmount.Currency })
            .Select(g => new
            {
                g.Key.JurisdictionId,
                g.Key.TaxPayableAccountId,
                g.Key.Currency,
                TotalTaxCollected = g.Sum(td => td.TaxAmount.Amount)
            })
            .ToList();

        var reportDtos = new List<TaxCollectedReportDto>();
        foreach (var item in groupedTaxData)
        {
            var jurisdiction = await jurisdictionRepository.GetByIdAsync(item.JurisdictionId, cancellationToken);
            var taxPayableAccountName = await glAccountRepository.GetAccountNameAsync(item.TaxPayableAccountId, cancellationToken);

            reportDtos.Add(new TaxCollectedReportDto(
                item.JurisdictionId,
                jurisdiction?.Name ?? "Unknown Jurisdiction",
                item.TaxPayableAccountId,
                taxPayableAccountName ?? "Unknown Tax Payable Account",
                new Money(item.TotalTaxCollected, item.Currency),
                request.StartDate,
                request.EndDate
            ));
        }

        return Result.Success(reportDtos.AsEnumerable());
    }
}