using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RecordPhysicalInventory;

public class RecordPhysicalInventoryCommand : IRequest<Result<Guid>>
{
    public Guid AssetId { get; set; }
    public DateTime CountDate { get; set; }
    public string CountedLocation { get; set; }
    public string CountedBy { get; set; }
    public string Notes { get; set; }
}