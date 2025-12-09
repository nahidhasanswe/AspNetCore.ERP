namespace ERP.Finance.Domain.Shared.Enums;

public enum InvoiceStatus
{
    Draft,
    Submitted,
    PendingApproval, // Added this
    Approved,
    ScheduledForPayment,
    Paid,
    PartiallyPaid,
    Cancel,
    Issued,
    Overdue,
    WrittenOff,
    Closed
}