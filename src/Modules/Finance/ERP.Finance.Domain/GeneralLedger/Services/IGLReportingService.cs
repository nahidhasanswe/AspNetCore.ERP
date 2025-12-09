using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.GeneralLedger.Services;

public interface IGLReportingService
{
    Task<Money> GetActualAmountForAccountAndPeriod(Guid accountId, string period, Guid? costCenterId, string currency);
    Task<Money> GetAccountBalanceAsOfDate(Guid accountId, DateTime asOfDate); // Added this
}