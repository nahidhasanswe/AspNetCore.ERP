namespace ERP.Finance.Domain.GeneralLedger.Services;

public interface IGLConfigurationService
{
    // Method to get the Tax Payable GL Account for a given jurisdiction and business unit
    Task<Guid> GetTaxPayableAccountId(Guid jurisdictionId, Guid businessUnitId);
    
    Task<Guid> GetBadDebtExpenseAccountId(string currency, CancellationToken cancellationToken = default);
    Task<Guid> GetUnappliedCashClearingAccountId(string currency, CancellationToken cancellationToken = default);

    Task<Guid> DebitMemoClearingAccountIdAsync(string currency, CancellationToken cancellationToken = default);
    Task<EncumbranceGlPostingDto?> GetEncumbranceGlAccounts(Guid glAccountId,
        CancellationToken cancellationToken = default);
    
    Task<Guid> GetAPControlAccountAsync(Guid businessUnitId, string currency, CancellationToken cancellationToken = default);
    
    Task<Guid> GetPaymentClearingAccountId(Guid businessUnitId, string currency, CancellationToken cancellationToken = default);
}

public record EncumbranceGlPostingDto(
    Guid EncumbranceAccountId,   // The GL account to DEBIT (e.g., 5500 - Encumbrance DR)
    Guid AppropriationAccountId // The GL account to CREDIT (e.g., 5501 - Appropriation CR)
);