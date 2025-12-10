using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxRate;

public class CreateTaxRateCommand : IRequest<Result<Guid>>
{
    public Guid JurisdictionId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; }
}