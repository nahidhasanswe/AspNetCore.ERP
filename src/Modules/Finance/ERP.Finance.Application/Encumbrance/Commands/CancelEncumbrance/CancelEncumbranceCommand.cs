using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CancelEncumbrance;

public class CancelEncumbranceCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid EncumbranceId { get; set; }
}