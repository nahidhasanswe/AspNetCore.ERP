using ERP.Core.Behaviors;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateRecurringJournal;

public class CreateRecurringJournalEntryCommand : IRequestCommand<Guid>
{
    public string Description { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
 
    public string Frequency { get; set; }
}
