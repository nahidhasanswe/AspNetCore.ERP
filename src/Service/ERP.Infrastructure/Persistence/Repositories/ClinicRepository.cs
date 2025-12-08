using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Specifications.Clinic;

namespace ERP.Infrastructure.Persistence.Repositories;

public class ClinicRepository(IDbContextProvider<BookingDbContext> dbContextProvider) : EfRepository<BookingDbContext, Clinic>(dbContextProvider), IClinicRepository
{
    public async Task<IEnumerable<Clinic>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ListAsync(new GetAllClinicSpecification(), cancellationToken);
    }

    public async Task<IEnumerable<Clinic>> GetByCity(string city, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new GetAllClinicByCitySpecification(city), cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await AnyAsync(new GetClinicByIdSpecification(id), cancellationToken);
    }
}