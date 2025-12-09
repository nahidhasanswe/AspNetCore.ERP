using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxRateById;

public class GetTaxRateByIdQuery : IRequest<Result<TaxRateDetailsDto>>
{
    public Guid TaxRateId { get; set; }
}