using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Queires;

public class GetAssetDetailsQuery : IRequest<Result<AssetDetailsDto>>
{
    public Guid AssetId { get; set; }
}