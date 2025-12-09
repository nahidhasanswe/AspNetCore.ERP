using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.FiscalYear.Enums;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Enums;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.PerformYearEndClose;

public class PerformYearEndCloseCommandHandler(
 IFiscalPeriodRepository fiscalPeriodRepository,
 IJournalEntryRepository journalEntryRepository,
 IGeneralLedgerEntryRepository generalLedgerEntryRepository,
 IAccountRepository accountRepository,
 IAccountValidationService accountValidationService,
 IUnitOfWorkManager unitOfWork
) : IRequestHandler<PerformYearEndCloseCommand, Result<Guid>>
{
 public async Task<Result<Guid>> Handle(PerformYearEndCloseCommand command, CancellationToken cancellationToken)
 {
     // 1. Get Fiscal Period and Validate Status
     var period = await fiscalPeriodRepository.GetByIdAsync(command.FiscalPeriodToCloseId, cancellationToken);
     if (period == null)
         return Result.Failure<Guid>("Fiscal period not found.");

     if (period.Status != PeriodStatus.SoftClose)
         return Result.Failure<Guid>("Fiscal period must be soft-closed before year-end close.");

     // 2. Get Retained Earnings Account
     var retainedEarningsAccount = await accountRepository.GetByIdAsync(command.RetainedEarningsAccountId, cancellationToken);
     if (retainedEarningsAccount == null)
         return Result.Failure<Guid>("Retained earnings account not found.");

     if (retainedEarningsAccount.Type != AccountType.Equity)
         return Result.Failure<Guid>("Retained earnings account must be an equity account.");

     // 3. Get all Revenue and Expense accounts
     var revenueAndExpenseAccounts = (await accountRepository.GetAllAsync(cancellationToken))
         .Where(a => a.Type == AccountType.Revenue || a.Type == AccountType.Expense)
         .ToList();

     // 4. Calculate the net income/loss for each account
     var accountBalances = await generalLedgerEntryRepository.GetAccountBalancesForPeriodAsync(period.StartDate, period.EndDate, cancellationToken);
     var balanceMap = accountBalances.ToDictionary(x => x.AccountId);
 
     var closingEntry = new JournalEntry($"Year-End Closing Entry for {period.Name}", $"YEC-{period.Name}", retainedEarningsAccount.BusinessUnitId);

     decimal netIncomeLoss = 0;

     foreach (var account in revenueAndExpenseAccounts)
     {
         var balance = balanceMap.TryGetValue(account.Id, out var b) ? b.Balance : 0;
         // Create lines to zero out the balance
         if (balance != 0)
         {
             // Debit if Credit balance, Credit if Debit balance
             bool isDebit = balance < 0;
             decimal absBalance = Math.Abs(balance);

             // Assume closing entries are in the system's base currency.
             var money = new Money(absBalance, "USD"); 

             closingEntry.AddLine(new LedgerLine(
                 closingEntry.Id,
                 account.Id,
                 money,
                 money,
                 isDebit,
                 $"Closing entry for {account.Name}"
             ));

             // Invert balance sign for net income calc (Credit balances for Revenue are negative here, but increase income)
             netIncomeLoss -= balance;
         }
     }

     // 5. Create a balancing entry to Retained Earnings
     // Credit if net income, Debit if net loss
     bool isCredit = netIncomeLoss > 0;
     decimal absNetIncomeLoss = Math.Abs(netIncomeLoss);

     var retainedEarningsMoney = new Money(absNetIncomeLoss, "USD"); 

     closingEntry.AddLine(new LedgerLine(
         closingEntry.Id,
         retainedEarningsAccount.Id,
         retainedEarningsMoney,
         retainedEarningsMoney,
         !isCredit,
         "Net income/loss to Retained Earnings"
     ));

     // 6. Post the closing entry
     // The closing entry must be posted into the period being closed, which is allowed when it is 'SoftClose'.
     // We pass 'isClosingEntry: true' to allow posting to a SoftClose period.
     closingEntry.Post(period, accountValidationService, isClosingEntry: true);
     period.HardClose(); // Now this will succeed because the period is still SoftClose.

     // 7. Persist the changes
     using var scope = unitOfWork.Begin();
     await journalEntryRepository.AddAsync(closingEntry, cancellationToken);
     await fiscalPeriodRepository.UpdateAsync(period, cancellationToken);
     await scope.SaveChangesAsync(cancellationToken);

     return Result.Success(closingEntry.Id);
 }
}