using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.Encumbrance.Queries.GetEncumbranceDetailsReport;

public class GetEncumbranceDetailsReportQuery : IRequest<Result<IEnumerable<EncumbranceDetailsDto>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}