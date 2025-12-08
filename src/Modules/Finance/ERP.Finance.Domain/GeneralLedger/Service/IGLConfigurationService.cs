namespace ERP.Finance.Domain.GeneralLedger.Service;

public interface IGLConfigurationService
{
    Task<Guid> GetBadDebtExpenseAccountId(string currency, CancellationToken cancellationToken = default);
    Task<Guid> GetUnappliedCashClearingAccountId(string currency, CancellationToken cancellationToken = default);

    Task<EncumbranceGLPostingDto?> GetEncumbranceGLAccounts(Guid glAccountId,
        CancellationToken cancellationToken = default);
}

public record EncumbranceGLPostingDto(
    Guid EncumbranceAccountId,   // The GL account to DEBIT (e.g., 5500 - Encumbrance DR)
    Guid AppropriationAccountId // The GL account to CREDIT (e.g., 5501 - Appropriation CR)
);