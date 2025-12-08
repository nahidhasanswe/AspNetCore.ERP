namespace ERP.Core.Policy.Permission.Models;

public class Permission
{
    public required string Action { get; set; }
    public string? Scope { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is not Permission other)
            return false;

        return Action == other.Action && Scope == other.Scope;
    }

    public override int GetHashCode()
    {
        return (Action, Scope).GetHashCode();
    }
}