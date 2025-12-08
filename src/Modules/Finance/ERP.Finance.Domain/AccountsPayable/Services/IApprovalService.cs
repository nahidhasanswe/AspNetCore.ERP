using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.AccountsPayable.Services;

public interface IApprovalService
{
    Task<bool> IsApprovalRequired(VendorInvoice invoice);
    Task<bool> HasSufficientApproval(VendorInvoice invoice, Guid approverId);
}