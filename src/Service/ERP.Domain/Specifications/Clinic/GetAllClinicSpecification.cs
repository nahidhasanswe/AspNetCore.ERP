using ERP.Core.Specifications;
using LinqKit;

namespace ERP.Domain.Specifications.Clinic;

public sealed class GetAllClinicSpecification : Specification<Aggregates.ClinicAggregate.Clinic>
{
    public GetAllClinicSpecification() : base(criteria => criteria.IsActive)
    {
        ApplyOrderBy(x => x.Name);
    }
}

public sealed class GetAllClinicByCitySpecification : Specification<Aggregates.ClinicAggregate.Clinic>
{
    public GetAllClinicByCitySpecification(string city) : base(criteria => criteria.IsActive && criteria.Address.City == city)
    {
        ApplyOrderBy(x => x.Name);
    }
}

public sealed class GetAllClinicWithPaginationSpecification : PagedListSpecification<Aggregates.ClinicAggregate.Clinic>
{
    public GetAllClinicWithPaginationSpecification(
            string? search = null,
            string? city = null,
            string? country = null,
            bool? active = null,
            int pageIndex = 0, 
            int pageSize = 50,
            string? sort = null
        ) : base(pageIndex, pageSize)
    {
        var predicate = PredicateBuilder.New<Aggregates.ClinicAggregate.Clinic>(true);

        if (!string.IsNullOrEmpty(search))
        {
            predicate.And(p => p.Name.ToLower().Contains(search.ToLower()));
        }

        if (!string.IsNullOrEmpty(city))
        {
            predicate.And(p => p.Address.City.ToLower() ==  city.ToLower());
        }
        
        if (!string.IsNullOrEmpty(country))
        {
            predicate.And(p => p.Address.Country.ToLower() ==  country.ToLower());
        }

        if (active is not null)
        {
            predicate.And(p => p.IsActive == active);
        }
        
        AddCriteria(predicate);
        
        if (!string.IsNullOrWhiteSpace(sort))
        {
            // name(default), address, city

            var sortExpr = sort.Split(' ');
            var descending = sortExpr switch
            {
                [_, "desc"] => true,
                _ => false
            };

            switch (sortExpr[0])
            {
                case "name":
                    ApplyOrderBy(x => x.Name, descending);
                    break;
                case "address":
                    ApplyOrderBy(x => x.Address.Street, descending);
                    break;
                case "city":
                    ApplyOrderBy(x => x.Address.City, descending);
                    break;
                default:
                    ApplyOrderBy(x => x.Name);
                    break;
            }
        }
        else
        {
            ApplyOrderBy(x => x.Name);
        }
        
        ApplyNoTracking();
    }
}