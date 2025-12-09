using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.TaxManagement.Queries.ListTaxRates;

public class ListTaxRatesQuery : IRequest<Result<IEnumerable<TaxRateSummaryDto>>>
{
    public Guid? JurisdictionId { get; set; }
    public DateTime? AsOfDate { get; set; } // To find rates effective on a specific date
    public bool? IsActive { get; set; }
}