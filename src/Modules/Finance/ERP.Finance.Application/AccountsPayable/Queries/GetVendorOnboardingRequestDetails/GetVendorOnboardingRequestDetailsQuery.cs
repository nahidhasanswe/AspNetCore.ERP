using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorOnboardingRequestDetails;

public class GetVendorOnboardingRequestDetailsQuery : IRequestCommand<VendorOnboardingRequestDetailsDto>
{
    public Guid RequestId { get; set; }
}