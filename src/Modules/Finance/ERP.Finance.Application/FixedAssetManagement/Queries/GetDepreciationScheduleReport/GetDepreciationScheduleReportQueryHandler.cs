using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.FixedAssetManagement.Enums;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.GetDepreciationScheduleReport;

public class GetDepreciationScheduleReportQueryHandler(IFixedAssetRepository fixedAssetRepository)
    : IRequestHandler<GetDepreciationScheduleReportQuery, Result<IEnumerable<DepreciationScheduleReportDto>>>
{
    public async Task<Result<IEnumerable<DepreciationScheduleReportDto>>> Handle(GetDepreciationScheduleReportQuery request, CancellationToken cancellationToken)
    {
        var assets = (await fixedAssetRepository.ListAllAsync(cancellationToken))
            .Where(asset => asset.Status == FixedAssetStatus.Active || asset.Status == FixedAssetStatus.UnderDepreciation);

        if (request.AssetId.HasValue)
        {
            assets = assets.Where(asset => asset.Id == request.AssetId.Value);
        }

        var reportLines = new List<DepreciationScheduleReportDto>();

        foreach (var asset in assets)
        {
            decimal currentAccumulatedDepreciation = asset.TotalAccumulatedDepreciation;
            decimal currentBookValue = asset.AcquisitionCost.Amount - currentAccumulatedDepreciation;
            DateTime currentPeriodDate = asset.AcquisitionDate;
            int periodCount = 0;

            // Simulate depreciation from acquisition date up to EndDate or UsefulLife
            while (currentPeriodDate <= (request.EndDate ?? DateTime.MaxValue) && periodCount < asset.Schedule.UsefulLifeYears)
            {
                if (currentPeriodDate >= (request.StartDate ?? DateTime.MinValue))
                {
                    // Calculate depreciation for this period
                    // This is a simplified simulation. In reality, you'd calculate for a specific month/year.
                    decimal annualDepreciation = asset.Schedule.CalculateAnnualDepreciation(asset.AcquisitionCost.Amount);
                    decimal monthlyDepreciation = annualDepreciation / 12m; // Simplified

                    // Ensure not to depreciate below salvage value
                    decimal depreciationForThisPeriod = Math.Min(monthlyDepreciation, asset.AcquisitionCost.Amount - asset.Schedule.SalvageValue - currentAccumulatedDepreciation);
                    
                    if (depreciationForThisPeriod > 0)
                    {
                        currentAccumulatedDepreciation += depreciationForThisPeriod;
                        currentBookValue = asset.AcquisitionCost.Amount - currentAccumulatedDepreciation;

                        reportLines.Add(new DepreciationScheduleReportDto(
                            asset.Id,
                            asset.TagNumber,
                            asset.Description,
                            currentPeriodDate, // Use the start of the month for simplicity
                            depreciationForThisPeriod,
                            currentAccumulatedDepreciation,
                            currentBookValue
                        ));
                    }
                }
                currentPeriodDate = currentPeriodDate.AddMonths(1); // Move to next month
                periodCount++; // This is a simplified period count, should be based on actual depreciation periods
            }
        }

        return Result.Success(reportLines.AsEnumerable());
    }
}