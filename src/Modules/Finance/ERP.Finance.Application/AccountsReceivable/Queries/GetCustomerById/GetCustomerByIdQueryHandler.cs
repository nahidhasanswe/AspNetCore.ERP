using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using ERP.Core.Mapping;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(
    ICustomerRepository customerRepository,
    ICustomerCreditProfileRepository creditProfileRepository,
    IObjectMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDetailsDto>>
{
    public async Task<Result<CustomerDetailsDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure<CustomerDetailsDto>("Customer not found.");
        }

        CustomerCreditProfileDto? creditProfileDto = null;
        if (customer.CustomerCreditProfileId.HasValue)
        {
            var creditProfile = await creditProfileRepository.GetByIdAsync(customer.CustomerCreditProfileId.Value, cancellationToken);
            if (creditProfile is not null)
            {
                creditProfileDto = new CustomerCreditProfileDto(
                    creditProfile.Id,
                    creditProfile.ApprovedLimit,
                    creditProfile.CurrentExposure,
                    creditProfile.Status,
                    creditProfile.DefaultPaymentTerms
                );
            }
        }

        var dto = new CustomerDetailsDto(
            customer.Id,
            customer.Name,
            customer.ContactEmail,
            mapper.Map<AddressDto>(customer.BillingAddress),
            mapper.Map<ContactInfoDto>(customer.ContactInfo),
            customer.PaymentTerms,
            customer.DefaultCurrency,
            customer.ARControlAccountId,
            customer.Status,
            customer.CustomerCreditProfileId,
            creditProfileDto
        );

        return Result.Success(dto);
    }
}