using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCashReceiptById;

public class GetCashReceiptByIdQueryHandler(
    ICashReceiptRepository cashReceiptRepository,
    ICustomerRepository customerRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetCashReceiptByIdQuery, Result<CashReceiptDetailsDto>>
{
    public async Task<Result<CashReceiptDetailsDto>> Handle(GetCashReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var cashReceipt = await cashReceiptRepository.GetByIdAsync(request.ReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure<CashReceiptDetailsDto>("Cash Receipt not found.");
        }

        var customerName = await customerRepository.GetNameByIdAsync(cashReceipt.CustomerId, cancellationToken);
        var cashAccountName = await glAccountRepository.GetAccountNameAsync(cashReceipt.CashAccountId, cancellationToken);

        var dto = new CashReceiptDetailsDto(
            cashReceipt.Id,
            cashReceipt.CustomerId,
            customerName ?? "Unknown Customer",
            cashReceipt.ReceiptDate,
            cashReceipt.TotalReceivedAmount,
            cashReceipt.TotalAppliedAmount,
            cashReceipt.UnappliedAmount,
            cashReceipt.TransactionReference,
            cashReceipt.CashAccountId,
            cashAccountName ?? "Unknown Cash Account",
            cashReceipt.Status
        );

        return Result.Success(dto);
    }
}