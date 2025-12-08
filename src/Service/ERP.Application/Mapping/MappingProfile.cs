using AutoMapper;
using ERP.Application.DTOs;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Aggregates.PatientAggregate;

namespace ERP.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Clinic
        CreateMap<Clinic, ClinicDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactInfo.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactInfo.Email));

        // Appointment
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Doctor.Specialization))
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name))
            .ForMember(dest => dest.AppointmentDateTime, opt => opt.MapFrom(src => src.TimeSlot.SlotStartDateTime))
            .ForMember(dest => dest.AppointmentEndDateTime, opt => opt.MapFrom(src => src.TimeSlot.SlotEndDateTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        
        
        // patient
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(d => d.Email, opt => opt.MapFrom(src => src.ContactInfo.Email))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.ContactInfo.Phone))
            .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address.Street));
        
        // TimeSlot
        CreateMap<Schedule, TimeSlotDto>()
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.SlotDurationMinutes));
        
        
        // Doctor
        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactInfo.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactInfo.Email))
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name));
    }
}