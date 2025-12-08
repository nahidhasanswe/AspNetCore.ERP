using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorInvoiceById;

public record GetVendorInvoiceByIdQuery(Guid InvoiceId) : IRequestCommand<VendorInvoiceDetailsDto>;