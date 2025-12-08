using ERP.Core.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IRecurringInvoiceRepository : IRepository<RecurringInvoice>
{
    Task<IEnumerable<RecurringInvoice>> GetActiveRecurringInvoicesDueForGenerationAsync(DateTime generationDate);
}