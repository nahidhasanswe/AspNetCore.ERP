using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;
using ERP.Finance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.AccountsPayable;

public class VendorRepository(IDbContextProvider<FinanceDbContext> dbContextProvider) : EfRepository<FinanceDbContext, Vendor>(dbContextProvider), IVendorRepository
{
    public Task<Vendor?> GetByTaxIdAsync(string taxId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetNameByIdAsync(Guid vendorId)
    {
        throw new NotImplementedException();
    }

    public Task<Vendor?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Vendor?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Vendor>> ListAllAsync()
    {
        throw new NotImplementedException();
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
}