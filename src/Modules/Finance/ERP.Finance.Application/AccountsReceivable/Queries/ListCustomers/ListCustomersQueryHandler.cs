using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCustomers;

public class ListCustomersQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<ListCustomersQuery, Result<IEnumerable<CustomerSummaryDto>>>
{
    public async Task<Result<IEnumerable<CustomerSummaryDto>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var allCustomers = await customerRepository.ListAllAsync(cancellationToken);

        var filteredCustomers = allCustomers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredCustomers = filteredCustomers.Where(c => c.Name.Contains(request.Name));
        }

        if (request.Status.HasValue)
        {
            filteredCustomers = filteredCustomers.Where(c => c.Status == request.Status.Value);
        }

        var summaryDtos = filteredCustomers.Select(c => new CustomerSummaryDto(
            c.Id,
            c.Name,
            c.ContactEmail,
            c.Status
        )).ToList();

        return Result.Success(summaryDtos.AsEnumerable());
    }
}