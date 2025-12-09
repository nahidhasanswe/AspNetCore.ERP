using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.DeactivateTaxRate;

public class DeactivateTaxRateCommand : IRequest<Result>
{
    public Guid TaxRateId { get; set; }
}