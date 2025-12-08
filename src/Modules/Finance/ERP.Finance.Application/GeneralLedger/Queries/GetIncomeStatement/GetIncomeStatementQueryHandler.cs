using ERP.Core;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Enums;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetIncomeStatement;

public class GetIncomeStatementQueryHandler(
    IGeneralLedgerEntryRepository ledgerEntryRepository,
    IAccountRepository accountRepository
) : IRequestHandler<GetIncomeStatementQuery, Result<IncomeStatementDto>>
{
    public async Task<Result<IncomeStatementDto>> Handle(GetIncomeStatementQuery request, CancellationToken cancellationToken)
    {
        // 1. Get all accounts to understand the hierarchy and types.
        var allAccounts = (await accountRepository.GetAllAsync(cancellationToken)).ToList();
        var accountMap = allAccounts.ToDictionary(a => a.Id);

        // 2. Get pre-aggregated balances ONLY for the relevant period from the database. This is highly performant.
        var balances = await ledgerEntryRepository.GetAccountBalancesForPeriodAsync(request.StartDate, request.EndDate, cancellationToken);

        var dto = new IncomeStatementDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        // 3. Process balances in memory (this is fast because the dataset is very small).
        foreach (var balance in balances)
        {
            if (!accountMap.TryGetValue(balance.AccountId, out var account)) continue;

            var line = new IncomeStatementLine
            {
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                // Note: Revenue has a natural credit balance. A negative result from (Debit-Credit) is positive revenue.
                Amount = account.Type == AccountType.Revenue ? -balance.Balance : balance.Balance,
                Level = 0 // Placeholder for hierarchy logic
            };

            if (account.Type == AccountType.Revenue)
                dto.Revenue.Add(line);
            else if (account.Type == AccountType.Expense)
                dto.Expenses.Add(line);
        }
        
        // TODO: Add logic here to sort and structure the lines hierarchically using the ParentId from the accounts.

        return Result.Success(dto);
    }
}