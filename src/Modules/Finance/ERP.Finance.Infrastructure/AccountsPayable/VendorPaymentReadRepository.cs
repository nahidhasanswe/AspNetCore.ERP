using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;


public class VendorPaymentReadRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) : EfRepository<FinanceDbContext, VendorInvoice>(dbContextProvider), IVendorPaymentReadRepository
{
    public async Task<List<VendorPaymentDto>> GetPaymentHistoryAsync(
        Guid vendorId, 
        DateTime? startDate, 
        DateTime? endDate)
    {
        // This is a simplified query that JOINs payment records with invoice data 
        // in an optimized way (e.g., using Dapper or a denormalized table).
        var query = Table
            .Where(p => p.VendorId == vendorId);
        
        if (startDate.HasValue)
        {
            query = query.Where(p => p.InvoiceDate >= startDate.Value);
        }
        
        query = query.Where(p => p.InvoiceDate <= endDate); // Use the calculated endDate

        // Execute and project to the DTO
        return await query.Select(p => new VendorPaymentDto
        {
            PaymentId = p.Id,
            PaymentDate = p.InvoiceDate,
            TransactionReference = p.InvoiceNumber,
            InvoiceNumber = p.InvoiceNumber, // Denormalized field
            AmountPaid = p.TotalAmount.Amount,
            Currency = p.TotalAmount.Currency,
            //PaymentMethod = p.PaymentMethod,
            Status = p.Status.ToString()
        }).ToListAsync();
    }
}