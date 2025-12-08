using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
}