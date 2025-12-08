using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;

public class CreateVendorCommandHandler(
    IVendorRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestHandler<CreateVendorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateVendorCommand command, CancellationToken cancellationToken)
    {
        // 1. Validation (Example: check for duplicate vendor name or Tax ID)
        var existingVendor = await repository.GetByNameAsync(command.Name, cancellationToken);
        if (existingVendor is not null)
        {
            return Result.Failure<Guid>($"A vendor with the name '{command.Name}' already exists.");
        }
        
        existingVendor = await repository.GetByTaxIdAsync(command.TaxId, cancellationToken);
        
        if (existingVendor is not null)
        {
            return Result.Failure<Guid>($"A vendor with the tax id '{command.TaxId}' already exists.");
        }
        

        // 2. Map DTOs to Domain Value Objects
        var address = new Address(command.Address.Street, command.Address.City, command.Address.State, command.Address.Country, command.Address.PostalCode);
        var contactInfo = new ContactInfo(command.ContactInfo.Phone, command.ContactInfo.Email);
        var bankDetails = new VendorBankDetails(command.BankDetails.AccountNumber, command.BankDetails.RoutingNumber, command.BankDetails.BankName, command.BankDetails.AccountName);

        // 3. Create the Aggregate Root
        var vendor = new Vendor(
            command.Name,
            command.TaxId,
            address,
            contactInfo,
            command.PaymentTerms,
            command.DefaultCurrency,
            bankDetails
        );

        // 4. Persist the new aggregate
        using var scope = unitOfWork.Begin();
        await repository.AddAsync(vendor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(vendor.Id);
    }
}