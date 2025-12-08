using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.HoldVendor;

public class HoldVendorInvoiceCommand : IRequestCommand<bool>
{
    public Guid InvoiceId { get; set; }
    public bool PlaceOnHold { get; set; } // True to hold, False to remove hold
}