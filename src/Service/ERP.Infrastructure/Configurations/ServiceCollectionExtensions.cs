using ERP.Core.EF;
using ERP.Core.EF.Uow;
using ERP.Core.Uow;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence.Repositories;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Aggregates.PatientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories
        (
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbOptionsBuilder
        )
    {
        services.AddDbContextPool<BookingDbContext>(dbOptionsBuilder);
        services.AddScoped<IDbContextProvider<BookingDbContext>, DefaultDbContextProvider<BookingDbContext>>();
        services.AddScoped<IUnitOfWorkManager, EfCoreUnitOfWorkManager<BookingDbContext>>();


        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        
        return services;
    }
}