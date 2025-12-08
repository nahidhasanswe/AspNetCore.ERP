using Microsoft.EntityFrameworkCore;

namespace ERP.Core.EF;

public interface IDbContextProvider<out TDbContext>
    where TDbContext : DbContext
{
    TDbContext GetDbContext();
}