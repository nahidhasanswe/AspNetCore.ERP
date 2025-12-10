using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FiscalYear.Commands.HardCloseFiscalPeriod;

public class HardCloseFiscalPeriodCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid FiscalPeriodId { get; set; }
}