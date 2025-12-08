namespace ERP.Application.DTOs;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime SlotDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
}