using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Core.Swagger.Attributes;

/// <summary>
/// Abstract base attribute used to store the response and request Type metadata.
/// This metadata is read by a custom IOperationFilter to generate Swagger/OpenAPI documentation.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
[Produces("application/json")]
// Common Error Responses (These are static and apply directly)
[ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)] 
[ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]  
[ProducesResponseType(typeof(void), (int)HttpStatusCode.InternalServerError)] 
public abstract class ApiOperationMetadataAttribute : Attribute
{
    // Properties to store the Types passed in the constructor
    public Type ResponseType { get; }
    public Type ExampleType { get; }
    public Type RequestType { get; protected set; }
    public Type RequestExampleType { get; protected set; }

    protected ApiOperationMetadataAttribute(Type responseType, Type exampleType)
    {
        ResponseType = responseType;
        ExampleType = exampleType;
    }
}