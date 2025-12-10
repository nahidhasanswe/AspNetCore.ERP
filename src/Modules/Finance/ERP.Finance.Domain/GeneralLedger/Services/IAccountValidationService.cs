namespace ERP.Finance.Domain.GeneralLedger.Services;

/// <summary>
/// A domain service to validate accounts before posting.
/// This would be implemented in the Application or Infrastructure layer with access to the Account repository.
/// </summary>
public interface IAccountValidationService
{
    void ValidatePostingAccount(Guid accountId);
    bool IsAccountInBusinessUnit(Guid accoountId, Guid businessUnitId);
}