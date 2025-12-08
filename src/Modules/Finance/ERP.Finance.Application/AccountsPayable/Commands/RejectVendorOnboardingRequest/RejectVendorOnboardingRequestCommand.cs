using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.RejectVendorOnboardingRequest;

public class RejectVendorOnboardingRequestCommand : IRequestCommand<Unit>
{
    public Guid RequestId { get; set; }
    public string Reason { get; set; }
}