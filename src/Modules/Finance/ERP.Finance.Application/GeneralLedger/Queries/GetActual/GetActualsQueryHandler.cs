using ERP.Core;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetActual;

public class GetActualsQueryHandler(
        IJournalEntryRepository repository
    ) : IRequestHandler<GetActualsQuery, Result<Dictionary<Guid, decimal>>>
{
    public async Task<Result<Dictionary<Guid, decimal>>> Handle(GetActualsQuery request, CancellationToken cancellationToken)
    {
        // Ensure the date range is inclusive of the start day and exclusive of the end day 
        // if using only date part, or adjust as necessary for your DateTime usage.
        var effectiveStartDate = request.StartDate.Date;
        var effectiveEndDate = request.EndDate.Date.AddDays(1).AddSeconds(-1); // End of the day

        var accountIdList = request.AccountIds.ToList();

        // 1. Query the database using LINQ
        // We join JournalEntries (the Aggregate Root) with LedgerLines (the inner entity)
        // and group the results by AccountId.
        var actualsData = await repository.QueryableData
            .AsNoTracking() // Read-only query for performance
            .Where(je => je.IsPosted && je.PostingDate >= effectiveStartDate && je.PostingDate <= effectiveEndDate)
            // Flatten the lines within the date range
            .SelectMany(je => je.Lines) 
            // Filter to only the accounts the Budgeting context cares about
            .Where(line => accountIdList.Contains(line.AccountId))
            .GroupBy(line => line.AccountId)
            .Select(g => new 
            {
                AccountId = g.Key,
                // The core calculation: Sum of Debits - Sum of Credits
                NetActual = g.Sum(line => line.IsDebit ? line.Amount.Amount : -line.Amount.Amount)
            })
            .ToDictionaryAsync(
                key => key.AccountId, 
                value => value.NetActual,
                cancellationToken
            );

        // 2. Add accounts that had zero activity (not returned by GroupBy)
        var result = new Dictionary<Guid, decimal>();
        
        foreach (var accountId in accountIdList)
        {
            // If the account had activity, use the calculated value. Otherwise, default to 0.
            if (actualsData.TryGetValue(accountId, out decimal netActual))
            {
                result.Add(accountId, netActual);
            }
            else
            {
                result.Add(accountId, 0m);
            }
        }

        return Result.Success(result);
    }
}