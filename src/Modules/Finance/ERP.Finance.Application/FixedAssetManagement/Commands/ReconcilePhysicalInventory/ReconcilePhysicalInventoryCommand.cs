using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ReconcilePhysicalInventory;

public class ReconcilePhysicalInventoryCommand : IRequest<Result>
{
    public Guid RecordId { get; set; }
}