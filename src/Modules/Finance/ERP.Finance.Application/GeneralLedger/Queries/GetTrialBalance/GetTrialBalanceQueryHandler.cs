using ERP.Core;
using ERP.Finance.Application.GeneralLedger.DTOs;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetTrialBalance;

public class GetTrialBalanceQueryHandler(
    IGeneralLedgerEntryRepository ledgerEntryRepository, // Assumes this repository exists
    IAccountRepository accountRepository
    ) : IRequestHandler<GetTrialBalanceQuery, Result<TrialBalanceDto>>
{
    public async Task<Result<TrialBalanceDto>> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
    {
        var allLedgerEntries = await ledgerEntryRepository.GetAllAsync(cancellationToken);
        var allAccounts = (await accountRepository.GetAllAsync(cancellationToken)).ToDictionary(a => a.Id);

        var balances = allLedgerEntries
            .GroupBy(e => e.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                Balance = g.Sum(e => e.Debit) - g.Sum(e => e.Credit)
            })
            .ToList();

        var dto = new TrialBalanceDto();

        foreach (var balance in balances)
        {
            if (!allAccounts.TryGetValue(balance.AccountId, out var account)) continue;

            var line = new TrialBalanceLineDto
            {
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                Debit = balance.Balance > 0 ? balance.Balance : 0,
                Credit = balance.Balance < 0 ? -balance.Balance : 0
            };
            dto.Lines.Add(line);
        }

        dto.TotalDebits = dto.Lines.Sum(l => l.Debit);
        dto.TotalCredits = dto.Lines.Sum(l => l.Credit);

        return Result.Success(dto);
    }
}