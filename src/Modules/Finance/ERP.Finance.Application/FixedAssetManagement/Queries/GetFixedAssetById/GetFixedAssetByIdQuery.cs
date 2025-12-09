using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.GetFixedAssetById;

public class GetFixedAssetByIdQuery : IRequest<Result<AssetDetailsDto>>
{
    public Guid AssetId { get; set; }
}