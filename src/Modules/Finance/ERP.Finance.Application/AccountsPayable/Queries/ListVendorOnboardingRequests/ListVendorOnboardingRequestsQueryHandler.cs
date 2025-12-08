using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Mapping;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorOnboardingRequests;

public class ListVendorOnboardingRequestsQueryHandler(
    IVendorOnboardingRequestRepository onboardingRepository,
    IObjectMapper mapper
    )
    : IRequestCommandHandler<ListVendorOnboardingRequestsQuery, IEnumerable<VendorOnboardingRequestSummaryDto>>
{
    public async Task<Result<IEnumerable<VendorOnboardingRequestSummaryDto>>> Handle(ListVendorOnboardingRequestsQuery request, CancellationToken cancellationToken)
    {
        var filteredRequests = await onboardingRepository.GetAllAsync(request.Status, cancellationToken);
        var summaryDtos = mapper.Map<List<VendorOnboardingRequestSummaryDto>>(filteredRequests);

        return Result.Success(summaryDtos.AsEnumerable());
    }
}