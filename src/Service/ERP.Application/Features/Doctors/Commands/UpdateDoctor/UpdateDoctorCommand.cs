using ERP.Application.Features.Doctors.Commands.CreateDoctor;

namespace ERP.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommand : CreateDoctorCommand
{
    public Guid Id { get; set; }
}
