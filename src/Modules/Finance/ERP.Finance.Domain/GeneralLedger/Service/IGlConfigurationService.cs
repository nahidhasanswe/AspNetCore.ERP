namespace ERP.Finance.Domain.GeneralLedger.Service;

public interface IGlConfigurationService
{
    Task<Guid> GetBadDebtExpenseAccountId(string currency, CancellationToken cancellationToken = default);
    Task<Guid> GetUnappliedCashClearingAccountId(string currency, CancellationToken cancellationToken = default);

    Task<Guid> DebitMemoClearingAccountIdAsync(string currency, CancellationToken cancellationToken = default);
    Task<EncumbranceGlPostingDto?> GetEncumbranceGlAccounts(Guid glAccountId,
        CancellationToken cancellationToken = default);
}

public record EncumbranceGlPostingDto(
    Guid EncumbranceAccountId,   // The GL account to DEBIT (e.g., 5500 - Encumbrance DR)
    Guid AppropriationAccountId // The GL account to CREDIT (e.g., 5501 - Appropriation CR)
);