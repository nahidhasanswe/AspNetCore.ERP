using ERP.Core.Repository;

namespace ERP.Domain.Aggregates.ClinicAggregate;

public interface IClinicRepository :  IRepository<Clinic>
{
    Task<IEnumerable<Clinic>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Clinic>> GetByCity(string city, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}