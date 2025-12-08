using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Domain.AccountsPayable.Service;

public interface IVendorStatementService
{
    /// <summary>
    /// Retrieves all outstanding (open) invoices for a vendor as of a specific date.
    /// </summary>
    Task<List<VendorStatementEntry>> GetOutstandingInvoicesAsync(Guid vendorId, DateTime asOfDate);

    /// <summary>
    /// Retrieves all payments and credit notes posted within a statement period.
    /// </summary>
    Task<List<VendorStatementEntry>> GetActivityForPeriodAsync(Guid vendorId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Generates a summary showing the beginning balance, activity, and ending balance.
    /// </summary>
    Task<VendorStatementSummary> GetStatementSummaryAsync(Guid vendorId, DateTime startDate, DateTime endDate);
}