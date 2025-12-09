using ERP.Core.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum RecurrenceInterval
{
    Monthly,
    Quarterly,
    Yearly
}

public class RecurringInvoice : AggregateRoot
{
    public Guid BusinessUnitId  {get; private set; }
    public Guid VendorId { get; private set; }
    public RecurrenceInterval Interval { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime NextOccurrenceDate { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<RecurringInvoiceLine> _lines = new();
    public IReadOnlyCollection<RecurringInvoiceLine> Lines => _lines.AsReadOnly();

    private RecurringInvoice() { }

    public static RecurringInvoice Create(Guid vendorId, Guid businessUnitId, RecurrenceInterval interval, DateTime startDate,
        DateTime? endDate, IEnumerable<RecurringInvoiceLine> lines)
    {
        var create = new RecurringInvoice
        {
            Id = Guid.NewGuid(),
            BusinessUnitId = businessUnitId,
            VendorId = vendorId,
            Interval = interval,
            StartDate = startDate,
            EndDate = endDate,
            NextOccurrenceDate = startDate,
            IsActive = true
        };
        
        create._lines.AddRange(lines);
        
        return create;
    }

    public void Update(RecurrenceInterval interval, DateTime startDate, DateTime? endDate, IEnumerable<RecurringInvoiceLine> newLines)
    {
        Interval = interval;
        StartDate = startDate;
        EndDate = endDate;
        
        _lines.Clear();
        _lines.AddRange(newLines);

        // Recalculate NextOccurrenceDate if StartDate changed and it's still active
        if (IsActive && NextOccurrenceDate < StartDate)
        {
            NextOccurrenceDate = StartDate;
        }
        // Further logic might be needed to adjust NextOccurrenceDate based on current date and new interval
    }

    public VendorInvoice GenerateInvoice(DateTime generationDate)
    {
        if (!IsActive || generationDate.Date != NextOccurrenceDate.Date)
            return null;

        var invoiceLines = new List<InvoiceLineItem>();
        foreach (var line in _lines)
        {
            invoiceLines.Add(new InvoiceLineItem(line.Description, line.LineAmount, line.ExpenseAccountId, line.CostCenterId));
        }

        var apControlAccountId = Guid.NewGuid(); // Placeholder
        var dueDate = generationDate.AddDays(30); // Placeholder due date logic

        var newInvoice = VendorInvoice.CreateNonPOInvoice(
            VendorId,
            this.BusinessUnitId,
            $"REC-{Id}-{generationDate:yyyyMMdd}",
            generationDate,
            dueDate,
            apControlAccountId,
            null,
            invoiceLines
        );

        UpdateNextOccurrenceDate();
        return newInvoice;
    }

    private void UpdateNextOccurrenceDate()
    {
        NextOccurrenceDate = Interval switch
        {
            RecurrenceInterval.Monthly => NextOccurrenceDate.AddMonths(1),
            RecurrenceInterval.Quarterly => NextOccurrenceDate.AddMonths(3),
            RecurrenceInterval.Yearly => NextOccurrenceDate.AddYears(1),
            _ => throw new InvalidOperationException("Invalid recurrence interval.")
        };

        if (EndDate.HasValue && NextOccurrenceDate > EndDate.Value)
        {
            IsActive = false;
        }
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

public class RecurringInvoiceLine
{
    public string Description { get; private set; }
    public Money LineAmount { get; private set; }
    public Guid ExpenseAccountId { get; private set; }
    public Guid? CostCenterId { get; private set; }

    public RecurringInvoiceLine(string description, Money lineAmount, Guid expenseAccountId, Guid? costCenterId)
    {
        Description = description;
        LineAmount = lineAmount;
        ExpenseAccountId = expenseAccountId;
        CostCenterId = costCenterId;
    }
}