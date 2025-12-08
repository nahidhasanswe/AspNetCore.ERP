using ERP.Core.Behaviors;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxJurisdiction;

public class CreateTaxJurisdictionCommand : IRequestCommand<Guid>
{
    public string Name { get; set; }
    public string RegionCode { get; set; }
}