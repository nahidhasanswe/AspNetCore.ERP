using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerInvoiceById;

public class GetCustomerInvoiceByIdQuery : IRequest<Result<CustomerInvoiceDetailsDto>>
{
    public Guid InvoiceId { get; set; }
}