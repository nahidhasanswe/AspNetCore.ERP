using ERP.Core.Behaviors;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTax;

public class CreateTaxRateCommand : IRequestCommand<Guid>
{
    public Guid JurisdictionId { get; set; }
    public decimal Rate { get; set; } // e.g., 0.0825
    public DateTime EffectiveDate { get; set; }
    public Guid TaxPayableAccountId { get; set; }
}