using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RunDepreciation;

public class RunDepreciationCommand : IRequest<Result>
{
    public DateTime PeriodDate { get; set; }
}