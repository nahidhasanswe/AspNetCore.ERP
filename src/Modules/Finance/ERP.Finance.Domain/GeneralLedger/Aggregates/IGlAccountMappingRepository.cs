using ERP.Core.Repository;
using System;
using System.Threading.Tasks;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public interface IGlAccountMappingRepository : IRepository<GlAccountMapping>
{
    Task<GlAccountMapping?> GetMapping(Guid businessUnitId, GlAccountMappingType mappingType, string currency, Guid? referenceId = null);
}