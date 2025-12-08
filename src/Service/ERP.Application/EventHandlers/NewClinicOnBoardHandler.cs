using ERP.Domain.Events;
using MediatR;

namespace ERP.Application.EventHandlers;

public class NewClinicOnBoardHandler : INotificationHandler<NewClinicRegistrationEvent>
{
    public Task Handle(NewClinicRegistrationEvent notification, CancellationToken cancellationToken)
    {
        var clinicId = notification.ClinicId;
        return Task.CompletedTask;
    }
}