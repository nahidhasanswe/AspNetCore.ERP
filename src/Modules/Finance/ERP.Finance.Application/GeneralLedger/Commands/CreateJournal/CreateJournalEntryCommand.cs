using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;

public class CreateJournalEntryCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; }
    public string Description { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime PostingDate { get; set; }
    public List<LedgerLineDto> Lines { get; set; } = new();

    public class LedgerLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool IsDebit { get; set; }
        public Guid? CostCenterId { get; set; }
    }
}