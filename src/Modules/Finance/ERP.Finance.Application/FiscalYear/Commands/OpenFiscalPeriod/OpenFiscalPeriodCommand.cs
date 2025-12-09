using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FiscalYear.Commands.OpenFiscalPeriod;

public class OpenFiscalPeriodCommand : IRequest<Result>
{
    public Guid FiscalPeriodId { get; set; }
}