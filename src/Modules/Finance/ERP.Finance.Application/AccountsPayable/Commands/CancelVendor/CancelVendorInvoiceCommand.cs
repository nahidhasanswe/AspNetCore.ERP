using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelVendor;

public class CancelVendorInvoiceCommand : IRequestCommand<bool>
{
    public Guid InvoiceId { get; set; }
    public string CancellationReason { get; set; }
}