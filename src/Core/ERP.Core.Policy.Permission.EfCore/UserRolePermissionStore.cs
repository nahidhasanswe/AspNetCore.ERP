using ERP.Core.Policy.Permission.Store;
using Microsoft.EntityFrameworkCore;

namespace ERP.Core.Policy.Permission.EfCore;

public class UserRolePermissionStore<TKey, TUserKey, TRoleKey, TUserPermission, TRolePermission, TResourcePermission, TContext> 
    : UserRolePermissionStoreBase<TKey, TUserKey, TRoleKey, TUserPermission, TRolePermission, TResourcePermission>

    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
    where TRoleKey : IEquatable<TRoleKey>
    where TUserPermission : UserPermission<TKey, TUserKey>, new()
    where TRolePermission : RolePermission<TKey, TRoleKey>, new()
    where TResourcePermission : ResourcePermission<TKey>, new()
    where TContext : DbContext

{
    public virtual TContext Context { get; private set; }


    public UserRolePermissionStore(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
    }
    
    private DbSet<TUserPermission> UserPermissionSet => Context.Set<TUserPermission>();
    private DbSet<TRolePermission> RolePermissionSet => Context.Set<TRolePermission>();
    private DbSet<TResourcePermission> ResourcePermissionSet => Context.Set<TResourcePermission>();
    
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>Saves the current store.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task SaveChanges(CancellationToken cancellationToken)
    {
        return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
    }
    
    public override async Task<Result> AddUserToPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userPermission);

        if (await IsUserInPermissionAsync(userId, userPermission, cancellationToken))
        {
            return Result.Failure("Resource permission already exists in this user");
        }
        
        var create = new TUserPermission()
        {
            UserId = ConvertIdFromString<TUserKey>(userId),
            ResourcePermissionId = userPermission.ResourcePermissionId
        };
        
        Context.Add(create);
        await SaveChanges(cancellationToken);
        return Result.Success();
    }

    public override async Task<Result> RemoveFromPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userPermission);
        
        var existing =
            await UserPermissionSet.FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.ResourcePermissionId.Equals(userPermission.ResourcePermissionId));

        if (existing is not null)
        {
            Context.Remove(existing);
            await SaveChanges(cancellationToken);
        }

        return Result.Success();
    }

    public override Task<bool> IsUserInPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userPermission);
        
        return UserPermissionSet.AnyAsync(x => x.UserId.Equals(userId) && x.ResourcePermissionId.Equals(userPermission.ResourcePermissionId));
    }

    public override async Task<IList<string>> GetUsersInPermissionAsync(TResourcePermission userPermission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userPermission);
        
        var userList = await UserPermissionSet.Where(x => x.ResourcePermissionId.Equals(userPermission.ResourcePermissionId))
            .Select(x => x.UserId).ToListAsync(cancellationToken);

        return userList.Select(x => ConvertIdToString<TUserKey>(x)).ToList();
    }

    public override async Task<IList<TResourcePermission>> GetPermissionsInUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(userId);

        var query = from userPermission in UserPermissionSet
            join resourcePermission in ResourcePermissionSet on userPermission.ResourcePermissionId equals
                resourcePermission.ResourcePermissionId
            select resourcePermission;

        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<Result> AddRoleToPermissionAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(rolePermission);

        if (await IsRoleInPermissionAsync(roleId, rolePermission, cancellationToken))
        {
            return Result.Failure("Resource permission already exists in this role");
        }
        
        var create = new TUserPermission()
        {
            UserId = ConvertIdFromString<TUserKey>(roleId),
            ResourcePermissionId = rolePermission.ResourcePermissionId
        };
        
        Context.Add(create);
        await SaveChanges(cancellationToken);
        return Result.Success();
    }

    public override async Task<Result> RemovePermissionFromRoleAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(rolePermission);
        
        var existing =
            await UserPermissionSet.FirstOrDefaultAsync(x => x.UserId.Equals(roleId) && x.ResourcePermissionId.Equals(rolePermission.ResourcePermissionId));

        if (existing is not null)
        {
            Context.Remove(existing);
            await SaveChanges(cancellationToken);
        }

        return Result.Success();
    }

    public override Task<bool> IsRoleInPermissionAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(rolePermission);
        
        return UserPermissionSet.AnyAsync(x => x.UserId.Equals(roleId) && x.ResourcePermissionId.Equals(rolePermission.ResourcePermissionId));
    }

    public override async Task<IList<string>> GetRolesInPermissionAsync(TResourcePermission rolePermission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(rolePermission);
        
        var userList = await RolePermissionSet.Where(x => x.ResourcePermissionId.Equals(rolePermission.ResourcePermissionId))
            .Select(x => x.RoleId).ToListAsync(cancellationToken);

        return userList.Select(x => ConvertIdToString<TRoleKey>(x)).ToList();
    }

    public override async Task<IList<TResourcePermission>> GetPermissionsInRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(roleId);

        var query = from rolePermission in RolePermissionSet
            join resourcePermission in ResourcePermissionSet on rolePermission.ResourcePermissionId equals
                resourcePermission.ResourcePermissionId
            select resourcePermission;

        return await query.ToListAsync(cancellationToken);
    }

    public override IQueryable<TUserPermission> UserPermissions => UserPermissionSet;
    public override IQueryable<TRolePermission> RolePermissions => RolePermissionSet;
}