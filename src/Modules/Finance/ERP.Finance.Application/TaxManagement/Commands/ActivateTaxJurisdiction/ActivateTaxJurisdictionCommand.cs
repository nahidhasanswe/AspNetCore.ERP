using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.ActivateTaxJurisdiction;

public class ActivateTaxJurisdictionCommand : IRequest<Result>
{
    public Guid JurisdictionId { get; set; }
}