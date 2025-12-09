using ERP.Core;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.GeneralLedger.Services;

public interface IGlPeriodStatusService
{
    Task<bool> IsPeriodOpenForPosting(DateTime date);
}