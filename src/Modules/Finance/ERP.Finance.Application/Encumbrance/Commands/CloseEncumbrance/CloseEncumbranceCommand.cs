using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CloseEncumbrance;

public class CloseEncumbranceCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } 
    public Guid EncumbranceId { get; set; }
}