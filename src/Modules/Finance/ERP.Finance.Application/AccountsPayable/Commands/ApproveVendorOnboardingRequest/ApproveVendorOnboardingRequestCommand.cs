using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApproveVendorOnboardingRequest;

public record ApproveVendorOnboardingRequestCommand(Guid RequestId, Guid ApproverId) : IRequestCommand<Unit>;