namespace ERP.Finance.Domain.AccountsReceivable.Service;

public interface IDeductionReasonService
{
    Task<Guid> GetExpenseAccountIdByReasonCode(string reasonCode, CancellationToken cancellationToken = default);
}