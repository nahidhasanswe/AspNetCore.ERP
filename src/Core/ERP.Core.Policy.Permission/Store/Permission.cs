namespace ERP.Core.Policy.Permission.Store;

public class Permission : Permission<string>
{
    public Permission()
    {
        Id = Guid.NewGuid().ToString();
    }

    public Permission(string scope, string action): this()
    {
        Scope = scope;
        Action = action;
    }
}

public class Permission<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
    public virtual string Action { get; set; } = default!;
    public virtual string Scope { get; set; } = default!;
    
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    
    public override string ToString()
    {
        return $"{Action}:{Scope}";
    }
}
