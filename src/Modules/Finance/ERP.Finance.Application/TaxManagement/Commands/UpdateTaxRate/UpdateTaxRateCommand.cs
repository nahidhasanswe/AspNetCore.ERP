using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.UpdateTaxRate;

public class UpdateTaxRateCommand : IRequest<Result>
{
    public Guid TaxRateId { get; set; }
    public decimal NewRate { get; set; }
    public DateTime NewEffectiveDate { get; set; }
    // Removed: public Guid NewTaxPayableAccountId { get; set; }
}