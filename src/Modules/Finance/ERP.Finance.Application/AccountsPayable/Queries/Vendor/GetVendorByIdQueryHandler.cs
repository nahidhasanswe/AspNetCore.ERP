using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Queries.Vendor;

public class GetVendorByIdQueryHandler(IVendorRepository repository)
    : IRequestCommandHandler<GetVendorByIdQuery, VendorDto>
{
    public async Task<Result<VendorDto>> Handle(GetVendorByIdQuery query, CancellationToken cancellationToken)
    {
        var vendor = await repository.GetByIdAsync(query.VendorId, cancellationToken);
        if (vendor == null) return Result.Failure<VendorDto>("Vendor not found.");

        // Map domain entity to DTO
        return Result.Success(new VendorDto
        (
            vendor.Id,
            vendor.Name,
            vendor.TaxId,
            vendor.PaymentTerms,
            vendor.ContactInfo.Email,
            vendor.ContactInfo.Phone
        ));
    }
}