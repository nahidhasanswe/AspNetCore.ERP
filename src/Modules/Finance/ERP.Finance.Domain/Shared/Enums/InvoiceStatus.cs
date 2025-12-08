namespace ERP.Finance.Domain.Shared.Enums;

public enum InvoiceStatus
{
    Draft,
    Submitted,
    Approved,
    ScheduledForPayment,
    Paid,
    Cancel,
    Issued,
    Overdue,
    WrittenOff,
    Closed
}