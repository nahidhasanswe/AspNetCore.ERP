using ERP.Application.Features.Clinics.Commands.CreateClinic;

namespace ERP.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommand : CreateClinicCommand
{
    public Guid Id { get; set; }
}