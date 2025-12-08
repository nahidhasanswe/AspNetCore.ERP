using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.DeactivateDoctor;

public class DeactivateDoctorCommand : IRequest<Result>
{
    public Guid DoctorId { get; set; }
}
