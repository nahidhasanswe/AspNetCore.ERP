using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.DeactivateVendor;

public class DeactivateVendorCommandHandler(IVendorRepository vendorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<DeactivateVendorCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeactivateVendorCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var vendor = await vendorRepository.GetByIdAsync(command.VendorId, cancellationToken);
        if (vendor == null)
        {
            return Result.Failure<Unit>("Vendor not found.");
        }

        vendor.Deactivate();

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}