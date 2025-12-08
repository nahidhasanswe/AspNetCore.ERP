using System.Globalization;
using FluentValidation;

namespace ERP.Application.Features.Doctors.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty().WithMessage("Doctor ID is required.");
        RuleFor(x => x.DayOfWeek).InclusiveBetween(0, 6).WithMessage("DayOfWeek must be between 0 (Sunday) and 6 (Saturday).");
        
        RuleFor(x => x.StartTime).Must(BeValidTimeSpan).WithMessage("Invalid StartTime format (HH:mm:ss).");
        RuleFor(x => x.EndTime).Must(BeValidTimeSpan).WithMessage("Invalid EndTime format (HH:mm:ss)")
            .GreaterThan(x => x.StartTime).When(x => BeValidTimeSpan(x.StartTime)).WithMessage("EndTime must be after StartTime.");
        
        RuleFor(x => x.SlotDurationMinutes)
            .InclusiveBetween(15, 120).WithMessage("Slot duration must be between 15 and 120 minutes.")
            .Must(m => m % 5 == 0).WithMessage("Slot duration must be a multiple of 5 minutes.");
        
        RuleFor(x => x.EffectiveFrom)
            .NotEmpty().WithMessage("EffectiveFrom date is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("EffectiveFrom date cannot be in the past.");
            
        RuleFor(x => x.EffectiveTo)
            .GreaterThan(x => x.EffectiveFrom).When(x => x.EffectiveTo.HasValue)
            .WithMessage("EffectiveTo must be after EffectiveFrom.");
    }
    
    private bool BeValidTimeSpan(string time) => TimeSpan.TryParse(time, CultureInfo.InvariantCulture, out _);
}