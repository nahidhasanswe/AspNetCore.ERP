using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class VendorInvoiceRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) : EfRepository<FinanceDbContext, VendorInvoice>(dbContextProvider), IVendorInvoiceRepository
{
    public override async ValueTask<VendorInvoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Table
            .Include(x => x.LineItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<VendorAging>> ListAllUnpaidAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // TODO: Convert all the calculation into database side
        var overdueInvoices = 
            from vi in Context.VendorInvoices
            join v in Context.Vendors on vi.VendorId equals v.Id
            where (vi.TotalAmount.Amount - vi.TotalPaymentsRecorded - vi.TotalCreditsApplied) > 0
            && vi.InvoiceDate <= asOfDate
            select new VendorAging
            {
                VendorId = vi.VendorId,
                VendorName = v.Name,
                OutstandingBalance = (vi.TotalAmount.Amount - vi.TotalPaymentsRecorded - vi.TotalCreditsApplied),
                DaysOverdue = (DateTime.Now - vi.DueDate).Days 
            };
        
        return await overdueInvoices.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CashFlowProjectionDto>> GetForecastProjectionAsync(DateTime dueDateCutoff, Guid? businessUnitId, CancellationToken cancellationToken = default)
    {
        var query = Context.VendorInvoices.AsQueryable();
        
        if (businessUnitId.HasValue) 
            query = query.Where(x => x.BusinessUnitId == businessUnitId);

        query = query.Where(x => x.DueDate <= dueDateCutoff);
        
        // TODO: Convert all the calculation into database side
        var overdueInvoices = 
            from vi in query
            join v in Context.Vendors on vi.VendorId equals v.Id
            select new CashFlowProjectionDto
            {
                VendorId = vi.VendorId,
                VendorName = v.Name,
                OutstandingBalance = vi.OutstandingBalance,
                DueDate = vi.DueDate,
                BusinessUnitId = vi.BusinessUnitId,
                InvoiceNumber = vi.InvoiceNumber,
                Status = vi.Status
            };
        
        return await overdueInvoices.OrderBy(x => x.DueDate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VendorSpendAnalysisDto>> GetSpendAnalysisListAsync(Guid? vendorId, InvoiceStatus? status, DateTime? dueDate, DateTime? startDate, DateTime? endDate,
        Guid? expenseAccountId, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        
        if (vendorId.HasValue)
            query = query.Where(x => x.VendorId == vendorId);
        
        if (status.HasValue)
            query = query.Where(x => x.Status == status);
        
        if (dueDate.HasValue)
            query = query.Where(x => x.DueDate == dueDate);
        
        if (startDate.HasValue && endDate.HasValue)
            query = query.Where(x => x.InvoiceDate >= startDate && x.InvoiceDate <= endDate);


        var result = await (from vi in query
            from line in vi.LineItems
            join v in Context.Vendors on vi.VendorId equals v.Id
            join acc in Context.Accounts on line.ExpenseAccountId equals acc.Id
            let item = new
            {
                VendorId = vi.Id,
                VendorName = v.Name,
                line.ExpenseAccountId,
                line.LineAmount.Amount,
                line.LineAmount.Currency,
                AccountName = acc.Name
            }
            where (expenseAccountId.HasValue && line.ExpenseAccountId == expenseAccountId)
            group item by new { item.VendorId, item.VendorName, item.AccountName, item.ExpenseAccountId, item.Currency }
            into g
            select new VendorSpendAnalysisDto 
                (
                    g.Key.VendorId,
                    g.Key.VendorName,
                    g.Key.ExpenseAccountId,
                    g.Key.AccountName,
                    g.Sum(x => x.Amount),
                    g.Key.Currency
                )
           ).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<VendorPaymentDto>> GetPaymentHistoryAsync(
        Guid vendorId, 
        DateTime? startDate, 
        DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        
        // TODO Optimized later
        var query = 
            from je in Context.JournalEntries
            join ll_debit in Context.LedgerLines on je.Id equals ll_debit.JournalEntryId
            join ll_credit in Context.LedgerLines on je.Id equals ll_credit.JournalEntryId
            join acc_debit in Context.Accounts on ll_debit.AccountId equals acc_debit.Id
            join acc_credit in Context.Accounts on ll_credit.AccountId equals acc_credit.Id
            where ll_debit.IsDebit && !ll_credit.IsDebit &&
                  acc_debit.Type == Domain.GeneralLedger.Enums.AccountType.Liability && // Debit to AP Control
                  acc_credit.Type == Domain.GeneralLedger.Enums.AccountType.Asset && // Credit from Cash/Bank
                  je.Description.Contains(vendorId.ToString()) // Simplified check
            select new VendorPaymentDto
            {
                PaymentId = je.Id,
                PaymentDate = je.PostingDate,
                TransactionReference = je.ReferenceNumber,
                InvoiceNumber = je.ReferenceNumber, // Assuming reference is invoice number
                AmountPaid = ll_credit.Amount.Amount,
                Currency = ll_credit.Amount.Currency,
                Status = je.IsPosted ? "Posted" : "Draft"
            };

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= endDate.Value);
        }

        return await query.OrderByDescending(p => p.PaymentDate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VendorInvoiceSummaryDto>> GetInvoiceSummaryAsync(Guid? vendorId,
        InvoiceStatus? status, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();

        if (vendorId.HasValue)
        {
            query = query.Where(x => x.VendorId == vendorId);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status);
        }

        return await query.Select(x => new VendorInvoiceSummaryDto
        (
            x.Id,
            x.InvoiceNumber,
            x.InvoiceDate,
            x.DueDate,
            x.TotalAmount.Amount,
            x.OutstandingBalance.Amount,
            x.Status
        )).ToListAsync<VendorInvoiceSummaryDto>(cancellationToken);
    }
}