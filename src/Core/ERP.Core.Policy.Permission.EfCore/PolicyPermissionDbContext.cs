using Microsoft.EntityFrameworkCore;

namespace ERP.Core.Policy.Permission.EfCore;

public class PolicyPermissionDbContext<TKey, TUserKey, TRoleKey, TResource, TPermission, TResourcePermission, TUserPermission, TRolePermission> :
    DbContext
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
    where TRoleKey : IEquatable<TRoleKey>
    where TResource : Store.Resource<TKey>
    where TPermission : Store.Permission<TKey>
    where TResourcePermission : Store.ResourcePermission<TKey>
    where TUserPermission : Store.UserPermission<TKey, TUserKey>
    where TRolePermission : Store.RolePermission<TKey, TRoleKey>
{
    public PolicyPermissionDbContext(DbContextOptions options) : base (options) { }
    
    protected PolicyPermissionDbContext() {}

    public virtual DbSet<TResource> Resources { get; set; } = default!;
    public virtual DbSet<TPermission> Permissions { get; set; } = default!;
    public virtual DbSet<TResourcePermission> ResourcePermissions { get; set; } = default!;
    public virtual DbSet<TUserPermission> UserPermissions { get; set; } = default!;
    public virtual DbSet<TRolePermission> RolePermissions { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TResource>(b =>
        {
            b.ToTable("Resources");
            b.HasKey(r => r.Id);
            b.HasIndex(r => r.Name).HasDatabaseName("IX_Resource_Name").IsUnique();
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.Name).HasMaxLength(256);

            b.HasMany<TResourcePermission>().WithOne().HasForeignKey(u => u.ResourceId).IsRequired();
        });
        
        modelBuilder.Entity<TPermission>(b =>
        {
            b.ToTable("Permissions");
            b.HasKey(r => r.Id);
            b.HasIndex(p => new { p.Action, p.Scope }).HasDatabaseName("IX_Permission_Action_Scope");
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.Action).HasMaxLength(256).IsRequired();
            b.Property(u => u.Scope).HasMaxLength(256).IsRequired(false);


            b.HasMany<TResourcePermission>().WithOne().HasForeignKey(u => u.PermissionId).IsRequired();
        });
        
        modelBuilder.Entity<TResourcePermission>(b =>
        {
            b.ToTable("ResourcePermissions");
            b.HasKey(r => r.ResourcePermissionId);
            b.HasIndex(p => new { p.ResourceId, p.PermissionId }).HasDatabaseName("IX_ResourcePermission_ResourceId_PermissionId");
        });
        
        modelBuilder.Entity<TUserPermission>(b =>
        {
            b.ToTable("UserPermissions");
            b.HasKey(r => new { r.UserId, r.ResourcePermissionId });
            b.HasIndex(p => new { p.UserId, p.ResourcePermissionId }).HasDatabaseName("IX_UserPermission_UserId_PermissionId");

            b.HasOne<TResourcePermission>().WithMany().HasForeignKey(u => u.ResourcePermissionId).IsRequired();
        });
        
        modelBuilder.Entity<TRolePermission>(b =>
        {
            b.ToTable("RolePermissions");
            b.HasKey(r => new { r.RoleId, r.ResourcePermissionId });
            b.HasIndex(p => new { p.RoleId, p.ResourcePermissionId }).HasDatabaseName("IX_RolePermission_RoleId_PermissionId");

            b.HasOne<TResourcePermission>().WithMany().HasForeignKey(u => u.ResourcePermissionId).IsRequired();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}