using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ERP.Finance.Infrastructure.Database;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    // Accounts Payable
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<VendorInvoice> VendorInvoices { get; set; }
    public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
    public DbSet<CreditMemo> CreditMemos { get; set; }
    public DbSet<DebitMemo> DebitMemos { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    public DbSet<RecurringInvoice> RecurringInvoices { get; set; }
    public DbSet<ApprovalRule> ApprovalRules { get; set; }
    public DbSet<VendorOnboardingRequest> VendorOnboardingRequests { get; set; }

    // Accounts Receivable
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerInvoice> CustomerInvoices { get; set; }
    public DbSet<CustomerInvoiceLineItem> CustomerInvoiceLineItems { get; set; }
    public DbSet<CustomerCreditMemo> CustomerCreditMemos { get; set; }
    public DbSet<CustomerCreditProfile> CustomerCreditProfiles { get; set; }
    public DbSet<CashReceipt> CashReceipts { get; set; }
    public DbSet<CashReceiptBatch> CashReceiptBatches { get; set; }

    // Budgeting
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetItem> BudgetItems { get; set; }
    public DbSet<BudgetApprovalRule> BudgetApprovalRules { get; set; }

    // Encumbrance
    public DbSet<Domain.Encumbrance.Aggregates.Encumbrance> Encumbrances { get; set; }

    // Fixed Asset Management
    public DbSet<FixedAsset> FixedAssets { get; set; }
    public DbSet<PhysicalInventoryRecord> PhysicalInventoryRecords { get; set; }
    public DbSet<AssetInsurancePolicy> AssetInsurancePolicies { get; set; }
    public DbSet<AssetMaintenanceRecord> AssetMaintenanceRecords { get; set; }
    public DbSet<LeasedAsset> LeasedAssets { get; set; }

    // Fiscal Year
    public DbSet<FiscalPeriod> FiscalPeriods { get; set; }

    // General Ledger
    public DbSet<Account> Accounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<LedgerLine> LedgerLines { get; set; }
    public DbSet<RecurringJournalEntry> RecurringJournalEntries { get; set; }
    public DbSet<GeneralLedgerEntry> GeneralLedgerEntries { get; set; }
    public DbSet<GlAccountMapping> GlAccountMappings { get; set; } // New DbSet for GlAccountMapping

    // Tax Management
    public DbSet<TaxJurisdiction> TaxJurisdictions { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}