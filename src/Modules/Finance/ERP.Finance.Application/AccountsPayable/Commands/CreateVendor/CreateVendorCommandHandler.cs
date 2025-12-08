using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;

public class CreateVendorCommandHandler(
    IVendorRepository repository,
    IUnitOfWorkManager unitOfWork
    )
    : IRequestCommandHandler<CreateVendorCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateVendorCommand command, CancellationToken cancellationToken)
    {
        // Domain Invariant: Check if vendor with the same Tax ID already exists
        if (await repository.GetByTaxIdAsync(command.TaxId) is not null)
            throw new DomainException($"Vendor with Tax ID {command.TaxId} already exists.");

        var vendor = new Vendor(command.Name, command.TaxId, command.ContactEmail, command.ContactPhone, command.PaymentTerms);

        using var scope = unitOfWork.Begin();
        await repository.AddAsync(vendor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(vendor.Id);
    }
}