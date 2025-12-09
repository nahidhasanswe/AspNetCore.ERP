using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxJurisdiction;

public class CreateTaxJurisdictionCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public string RegionCode { get; set; }
}