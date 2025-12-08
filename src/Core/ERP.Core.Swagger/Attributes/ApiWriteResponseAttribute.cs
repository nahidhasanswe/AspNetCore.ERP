namespace ERP.Core.Swagger.Attributes;

/// <summary>
/// Single attribute for WRITE operations (e.g., POST/PUT).
/// </summary>
public class ApiWriteResponseAttribute : ApiOperationMetadataAttribute
{
    /// <param name="requestType">The Request Model sent in the body (e.g., typeof(Domain.Area)).</param>
    /// <param name="requestExampleType">The Swagger Request Example model (e.g., typeof(AreaUpdateExample)).</param>
    /// <param name="responseType">The Domain model returned on success (e.g., typeof(Domain.Area)).</param>
    /// <param name="exampleType">The Swagger Response Example model (e.g., typeof(AreaExample)).</param>
    public ApiWriteResponseAttribute(
        Type requestType, 
        Type requestExampleType, 
        Type responseType, 
        Type exampleType) 
        : base(responseType, exampleType)
    {
        RequestType = requestType;
        RequestExampleType = requestExampleType;
    }
}