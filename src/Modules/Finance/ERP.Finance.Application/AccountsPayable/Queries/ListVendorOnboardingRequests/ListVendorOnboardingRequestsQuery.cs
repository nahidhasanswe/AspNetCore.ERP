using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorOnboardingRequests;

public class ListVendorOnboardingRequestsQuery : IRequestCommand<IEnumerable<VendorOnboardingRequestSummaryDto>>
{
    public OnboardingStatus? Status { get; set; }
}