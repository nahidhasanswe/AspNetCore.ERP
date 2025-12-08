using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApproveVendorOnboardingRequest;

public class ApproveVendorOnboardingRequestCommandHandler(
    IVendorOnboardingRequestRepository onboardingRepository,
    IMediator mediator,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ApproveVendorOnboardingRequestCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ApproveVendorOnboardingRequestCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var request = await onboardingRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure<Unit>("Vendor onboarding request not found.");
        }

        var address = new AddressDto(request.ProposedAddress.Street, request.ProposedAddress.City,
            request.ProposedAddress.State, request.ProposedAddress.Country, request.ProposedAddress.PostalCode);
        
        var contactInfo = new ContactInfoDto(request.ProposedContactInfo.Email, request.ProposedContactInfo.Phone);
        var bankDetails = new VendorBankDetailsDto(request.ProposedBankDetails.AccountNumber, request.ProposedBankDetails.RoutingNumber, request.ProposedBankDetails.BankName, request.ProposedBankDetails.AccountNumber);

        // Create the actual Vendor record
        var createVendorCommand = new CreateVendorCommand
        {
            Name = request.ProposedName,
            TaxId = request.ProposedTaxId,
            Address = address,
            ContactInfo = contactInfo,
            PaymentTerms = request.ProposedPaymentTerms,
            DefaultCurrency = request.ProposedDefaultCurrency,
            BankDetails = bankDetails
        };

        var createVendorResult = await mediator.Send(createVendorCommand, cancellationToken);
        if (createVendorResult.IsFailure)
        {
            return Result.Failure<Unit>($"Failed to create vendor: {createVendorResult.Error}");
        }

        request.Approve(createVendorResult.Value); // Pass the newly created VendorId
        request.MarkAsCompleted(); // Mark the request as completed

        await onboardingRepository.UpdateAsync(request, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}