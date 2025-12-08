namespace ERP.Core.Swagger.Attributes;

/// <summary>
/// Single attribute for READ operations (e.g., GET).
/// </summary>
public class ApiReadResponseAttribute : ApiOperationMetadataAttribute
{
    /// <param name="responseType">The Domain model returned (e.g., typeof(Domain.Area)).</param>
    /// <param name="exampleType">The Swagger Response Example model (e.g., typeof(AreaExample)).</param>
    public ApiReadResponseAttribute(Type responseType, Type exampleType) 
        : base(responseType, exampleType) { }
}
