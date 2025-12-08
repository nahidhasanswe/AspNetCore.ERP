using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendor;

public class UpdateVendorCommandHandler(IVendorRepository vendorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<UpdateVendorCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateVendorCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var vendor = await vendorRepository.GetByIdAsync(command.VendorId, cancellationToken);
        if (vendor == null)
        {
            return Result.Failure<Unit>("Vendor not found.");
        }

        // Map DTOs to Value Objects
        var address = new Address(
            command.Address.Street,
            command.Address.City,
            command.Address.State,
            command.Address.PostalCode,
            command.Address.Country
        );

        var contactInfo = new ContactInfo(
            command.ContactInfo.Email,
            command.ContactInfo.Phone
        );

        var bankDetails = new VendorBankDetails(
            command.BankDetails.BankName,
            command.BankDetails.AccountNumber,
            command.BankDetails.RoutingNumber,
            command.BankDetails.AccountName
        );

        vendor.UpdateInfo(
            command.Name,
            command.TaxId,
            address,
            contactInfo,
            command.PaymentTerms,
            command.DefaultCurrency,
            bankDetails,
            command.IsActive
        );

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}