using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxJurisdictionById;

public class GetTaxJurisdictionByIdQuery : IRequest<Result<TaxJurisdictionDetailsDto>>
{
    public Guid JurisdictionId { get; set; }
}