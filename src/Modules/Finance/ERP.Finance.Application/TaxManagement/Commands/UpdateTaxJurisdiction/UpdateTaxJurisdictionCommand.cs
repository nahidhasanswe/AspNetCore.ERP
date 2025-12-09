using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.UpdateTaxJurisdiction;

public class UpdateTaxJurisdictionCommand : IRequest<Result>
{
    public Guid JurisdictionId { get; set; }
    public string NewName { get; set; }
    public string NewRegionCode { get; set; }
}