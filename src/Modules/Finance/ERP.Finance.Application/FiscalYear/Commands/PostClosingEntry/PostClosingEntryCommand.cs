using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FiscalYear.Commands.PostClosingEntry;

public class PostClosingEntryCommand : IRequest<Result>
{
    public Guid FiscalPeriodId { get; set; }
}