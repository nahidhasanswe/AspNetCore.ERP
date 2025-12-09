using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCashReceipts;

public class ListCashReceiptsQueryHandler(
    ICashReceiptRepository cashReceiptRepository,
    ICustomerRepository customerRepository)
    : IRequestHandler<ListCashReceiptsQuery, Result<IEnumerable<CashReceiptSummaryDto>>>
{
    public async Task<Result<IEnumerable<CashReceiptSummaryDto>>> Handle(ListCashReceiptsQuery request, CancellationToken cancellationToken)
    {
        var allCashReceipts = await cashReceiptRepository.ListAllAsync(cancellationToken);

        var filteredReceipts = allCashReceipts.AsQueryable();

        if (request.CustomerId.HasValue)
        {
            filteredReceipts = filteredReceipts.Where(cr => cr.CustomerId == request.CustomerId.Value);
        }

        if (request.Status.HasValue)
        {
            filteredReceipts = filteredReceipts.Where(cr => cr.Status == request.Status.Value);
        }

        if (request.StartDate.HasValue)
        {
            filteredReceipts = filteredReceipts.Where(cr => cr.ReceiptDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            filteredReceipts = filteredReceipts.Where(cr => cr.ReceiptDate <= request.EndDate.Value);
        }

        var summaryDtos = new List<CashReceiptSummaryDto>();
        foreach (var cr in filteredReceipts)
        {
            var customerName = await customerRepository.GetNameByIdAsync(cr.CustomerId);
            summaryDtos.Add(new CashReceiptSummaryDto(
                cr.Id,
                cr.CustomerId,
                customerName ?? "Unknown Customer",
                cr.ReceiptDate,
                cr.TotalReceivedAmount,
                cr.UnappliedAmount,
                cr.Status
            ));
        }

        return Result.Success(summaryDtos.AsEnumerable());
    }
}