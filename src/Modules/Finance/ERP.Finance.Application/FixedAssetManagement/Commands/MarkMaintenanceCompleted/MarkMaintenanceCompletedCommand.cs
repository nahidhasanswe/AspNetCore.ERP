using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.MarkMaintenanceCompleted;

public class MarkMaintenanceCompletedCommand : IRequest<Result>
{
    public Guid MaintenanceRecordId { get; set; }
    public DateTime CompletionDate { get; set; }
}