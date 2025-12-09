using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.GetDepreciationScheduleReport;

public class GetDepreciationScheduleReportQuery : IRequest<Result<IEnumerable<DepreciationScheduleReportDto>>>
{
    public Guid? AssetId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}