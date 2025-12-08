namespace ERP.Application.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public DateTime AppointmentDateTime { get; set; }
    public DateTime AppointmentEndDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string? Notes { get; set; }
}