using ERP.Core;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;
using MediatR;
using System;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsPayable.Commands.SubmitVendorOnboardingRequest;

public class SubmitVendorOnboardingRequestCommand : IRequest<Result<Guid>>
{
    public string ProposedName { get; set; }
    public string ProposedTaxId { get; set; }
    public Address ProposedAddress { get; set; }
    public ContactInfo ProposedContactInfo { get; set; }
    public string ProposedPaymentTerms { get; set; }
    public string ProposedDefaultCurrency { get; set; }
    public VendorBankDetails ProposedBankDetails { get; set; }
}