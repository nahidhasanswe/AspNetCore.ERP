using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.Encumbrance.Commands.CloseEncumbrance;

public class CloseEncumbranceCommand : IRequest<Result>
{
    public Guid EncumbranceId { get; set; }
}