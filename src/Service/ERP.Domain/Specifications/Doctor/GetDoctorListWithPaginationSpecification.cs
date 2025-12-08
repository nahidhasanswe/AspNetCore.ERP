using ERP.Core.Specifications;
using System.Linq.Expressions;
using LinqKit;

namespace ERP.Domain.Specifications.Doctor;

public class GetDoctorListWithPaginationSpecification : PagedListSpecification<Aggregates.DoctorAggregate.Doctor>
{
    public GetDoctorListWithPaginationSpecification(
        string? search,
        string? specialization,
        Guid? clinicId,
        bool? active,
        int pageIndex,
        int pageSize,
        string? sort)
        : base(pageIndex, pageSize)
    {

        var query = PredicateBuilder.New<Aggregates.DoctorAggregate.Doctor>(true);
        // Search criteria
        if (!string.IsNullOrWhiteSpace(search))
        {
            Expression<Func<Aggregates.DoctorAggregate.Doctor, bool>> searchExpression = d =>
                d.FirstName.Contains(search) ||
                d.LastName.Contains(search) ||
                d.Specialization.Contains(search) ||
                d.LicenseNumber.Contains(search);
            query.And(searchExpression);
        }

        // Specialization filter
        if (!string.IsNullOrWhiteSpace(specialization))
        {
            query.And(d => d.Specialization.Contains(specialization));
        }

        // Clinic ID filter
        if (clinicId.HasValue && clinicId != Guid.Empty)
        {
            query.And(d => d.ClinicId == clinicId.Value);
        }

        // Active status filter
        if (active.HasValue)
        {
            query.And(d => d.IsActive == active.Value);
        }
        
        if (!string.IsNullOrEmpty(sort))
        {
            // name(default), address, city

            var sortExpr = sort.Split(' ');
            var descending = sortExpr switch
            {
                [_, "desc"] => true,
                _ => false
            };

            var columnName = sortExpr[0];

            Expression<Func<Aggregates.DoctorAggregate.Doctor, object>> orderBy = columnName switch
            {
                "name" => x => x.FirstName,
                "phone" => x => x.ContactInfo.Phone,
                "email" => x => x.ContactInfo.Email,
                "specialization" => x => x.Specialization,
                "license" => x => x.LicenseNumber,
                "active" => x => x.IsActive,
                _ => x => x.FirstName
            };

            ApplyOrderBy(orderBy, descending);
        }
        else
        {
            ApplyOrderBy(x => x.FirstName);
        }
        
        AddCriteria(query);
    }
}
