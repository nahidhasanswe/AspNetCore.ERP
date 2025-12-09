using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ScheduleAssetMaintenance;

public class ScheduleAssetMaintenanceCommand : IRequest<Result<Guid>>
{
    public Guid AssetId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Description { get; set; }
    public Money Cost { get; set; }
    public string PerformedBy { get; set; }
}