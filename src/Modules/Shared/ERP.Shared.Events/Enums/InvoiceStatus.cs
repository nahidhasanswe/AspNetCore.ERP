namespace ERP.Shared.Events.Enums;

public enum InvoiceStatus
{
    Draft,
    Submitted,
    PendingApproval,
    Approved,
    ScheduledForPayment,
    Paid,
    Cancel,
    Issued,
    Overdue,
    WrittenOff,
    Closed
}