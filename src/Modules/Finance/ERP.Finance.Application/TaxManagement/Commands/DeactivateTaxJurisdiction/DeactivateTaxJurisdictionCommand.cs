using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.DeactivateTaxJurisdiction;

public class DeactivateTaxJurisdictionCommand : IRequest<Result>
{
    public Guid JurisdictionId { get; set; }
}