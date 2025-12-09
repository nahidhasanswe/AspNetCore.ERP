using ERP.Finance.Application.FixedAssetManagement.Commands.CreateFixedAsset;
using ERP.Finance.Domain.AccountsPayable.Events; // Event from AP module
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.EventHandlers;

public class FixedAssetAcquiredViaInvoiceEventHandler(IMediator mediator, ILogger<FixedAssetAcquiredViaInvoiceEventHandler> logger)
    : INotificationHandler<FixedAssetAcquiredViaInvoiceEvent>
{
    public async Task Handle(FixedAssetAcquiredViaInvoiceEvent notification, CancellationToken cancellationToken)
    {
        var createAssetCommand = new CreateFixedAssetCommand
        {
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            TagNumber = notification.AssetTagNumber,
            Description = notification.AssetDescription,
            AcquisitionDate = notification.AcquisitionDate,
            AcquisitionCost = notification.AcquisitionCost,
            AssetAccountId = notification.AssetAccountId,
            DepreciationExpenseAccountId = notification.DepreciationExpenseAccountId,
            AccumulatedDepreciationAccountId = notification.AccumulatedDepreciationAccountId,
            DepreciationMethod = notification.DepreciationMethod,
            UsefulLifeYears = notification.UsefulLifeYears,
            SalvageValue = notification.SalvageValue,
            CostCenterId = notification.CostCenterId
        };

        // Send the command to create the fixed asset
        var result = await mediator.Send(createAssetCommand, cancellationToken);

        if (result.IsFailure)
        {
            // Log the error: Failed to create fixed asset from invoice event
            logger.LogError($"Failed to create fixed asset for invoice {notification.InvoiceId}: {result.Error}");
        }
        else
        {
            logger.LogInformation($"Successfully created fixed asset {result.Value} from invoice {notification.InvoiceId}.");
        }
    }
}