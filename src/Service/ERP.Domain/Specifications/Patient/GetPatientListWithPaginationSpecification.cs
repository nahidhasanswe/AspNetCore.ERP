using ERP.Core.Specifications;
using LinqKit;

namespace ERP.Domain.Specifications.Patient;

public sealed class GetPatientListWithPaginationSpecification : PagedListSpecification<Aggregates.PatientAggregate.Patient>
{
    public GetPatientListWithPaginationSpecification(
            string? search = null,
            string? gender = null,
            string? city = null,
            string? country = null,
            DateTime? fromDateOfBirth = null,
            DateTime? toDateOfBirth = null,
            int pageIndex = 0, 
            int pageSize = 50,
            string? sort = null
        ): base(pageIndex, pageSize)
    {
        var predicate = PredicateBuilder.New<Aggregates.PatientAggregate.Patient>(true);

        if (!string.IsNullOrEmpty(search))
        {
            predicate.And(p => p.FirstName.Contains(search) || p.LastName.Contains(search));
        }

        if (!string.IsNullOrEmpty(gender))
        {
            predicate.And(p => p.Gender.ToLower() ==  gender.ToLower());
        }

        if (!string.IsNullOrEmpty(city))
        {
            predicate.And(p => p.Address.City.ToLower() ==  city.ToLower());
        }
        
        if (!string.IsNullOrEmpty(country))
        {
            predicate.And(p => p.Address.Country.ToLower() ==  country.ToLower());
        }

        if (fromDateOfBirth.HasValue && toDateOfBirth.HasValue)
        {
            predicate.And(p => p.DateOfBirth >= fromDateOfBirth && p.DateOfBirth <= toDateOfBirth);
        }
        
        AddCriteria(predicate);
        
        if (!string.IsNullOrEmpty(sort))
        {
            // name(default), dateOfBirth, gender

            var sortExpr = sort.Split(' ');
            var descending = sortExpr switch
            {
                [_, "desc"] => true,
                _ => false
            };

            switch (sortExpr[0])
            {
                case "name":
                    ApplyOrderBy(x => x.FirstName, descending);
                    break;
                case "dateOfBirth":
                    ApplyOrderBy(x => x.FirstName, descending);
                    break;
                case "gender":
                    ApplyOrderBy(x => x.FirstName, descending);
                    break;
                default:
                    ApplyOrderBy(x => x.FirstName);
                    break;
            }
        }
        else
        {
            ApplyOrderBy(x => x.FirstName);
        }
        
        ApplyNoTracking();
    }
}