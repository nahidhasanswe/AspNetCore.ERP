using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.Vendor;

public class GetVendorByIdQuery : IRequestCommand<VendorDto>
{
    public Guid VendorId { get; set; }
}