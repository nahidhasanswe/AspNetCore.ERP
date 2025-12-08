using ERP.Core;
using MediatR;

namespace ERP.Application.Features.Patient.Commands.UpdatePatient;

public class UpdatePatientCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty; // e.g., "Male", "Female", "Other"
    
    // ContactInfo Value Object properties
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Address Value Object properties (optional)
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    
    public string? EmergencyContact { get; set; }
}