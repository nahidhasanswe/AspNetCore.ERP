using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.GeneralLedger.Services;

public interface IGLConfigurationService
{
    // Method to get the Tax Payable GL Account for a given jurisdiction and business unit
    Task<Guid> GetTaxPayableAccountId(Guid jurisdictionId, Guid businessUnitId);
    // Potentially other configuration methods like GetARControlAccount, GetAPControlAccount etc.
}