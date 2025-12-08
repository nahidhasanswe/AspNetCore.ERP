namespace ERP.Core.Policy.Permission.Store;


public class Resource : Resource<string>
{
    public Resource()
    {
        Id = Guid.NewGuid().ToString();
    }

    public Resource(string name) : this()
    {
        Name = name;
    }
}

public class Resource<TKey>
    where TKey : IEquatable<TKey>
{
    public Resource() { }

    public Resource(string name) : this()
    {
        Name = name;
    }

    public virtual TKey Id { get; set; } = default!;
    public virtual string Name { get; set; } = default!;

    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    
    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}