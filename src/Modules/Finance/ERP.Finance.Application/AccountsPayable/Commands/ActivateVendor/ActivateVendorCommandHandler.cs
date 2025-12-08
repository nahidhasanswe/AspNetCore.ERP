using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ActivateVendor;

public class ActivateVendorCommandHandler(IVendorRepository vendorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ActivateVendorCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ActivateVendorCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var vendor = await vendorRepository.GetByIdAsync(command.VendorId, cancellationToken);
        if (vendor == null)
        {
            return Result.Failure<Unit>("Vendor not found.");
        }

        vendor.Activate();

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}