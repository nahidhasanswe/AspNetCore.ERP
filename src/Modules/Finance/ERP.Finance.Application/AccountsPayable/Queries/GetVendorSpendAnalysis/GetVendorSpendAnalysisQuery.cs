using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorSpendAnalysis;

public class GetVendorSpendAnalysisQuery : IRequest<Result<IEnumerable<VendorSpendAnalysisDto>>>
{
    public Guid? VendorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ExpenseAccountId { get; set; }
}