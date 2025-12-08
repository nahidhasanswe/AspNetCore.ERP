namespace ERP.Core.Behaviors;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class EnableTransactionAttribute : Attribute
{
    // Required parameter defined in the constructor
    public bool Enabled { get; } = false;

    /// <summary>
    /// Initializes a new instance of the EnableTransactionAttribute.
    /// </summary>
    /// <param name="isEnabled">A required flag for the operation.</param>
    public EnableTransactionAttribute(bool isEnabled)
    {
        Enabled = isEnabled;
    }
}