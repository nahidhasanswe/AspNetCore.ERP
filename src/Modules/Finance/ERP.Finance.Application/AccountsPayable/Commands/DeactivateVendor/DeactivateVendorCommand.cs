using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.DeactivateVendor;

public record DeactivateVendorCommand(Guid VendorId) : IRequestCommand<Unit>;