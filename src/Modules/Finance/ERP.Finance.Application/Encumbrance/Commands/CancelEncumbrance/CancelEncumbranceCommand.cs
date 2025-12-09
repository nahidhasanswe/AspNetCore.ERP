using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CancelEncumbrance;

public class CancelEncumbranceCommand : IRequest<Result>
{
    public Guid EncumbranceId { get; set; }
}