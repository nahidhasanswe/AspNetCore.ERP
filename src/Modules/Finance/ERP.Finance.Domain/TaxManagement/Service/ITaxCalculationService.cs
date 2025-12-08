using ERP.Finance.Domain.TaxManagement.DTOs;

namespace ERP.Finance.Domain.TaxManagement.Service;

public interface ITaxCalculationService
{
    Task<TaxCalculationResult> CalculateTax(TaxCalculationRequest request);
}