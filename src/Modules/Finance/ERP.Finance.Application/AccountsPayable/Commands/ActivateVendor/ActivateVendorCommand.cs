using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.ActivateVendor;

public record ActivateVendorCommand(Guid VendorId) : IRequestCommand<Unit>;