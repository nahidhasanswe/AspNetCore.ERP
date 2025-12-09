using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxRate;

public class CreateTaxRateCommand : IRequest<Result<Guid>>
{
    public Guid JurisdictionId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public Guid TaxPayableAccountId { get; set; }
}