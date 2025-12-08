using System.Reflection;
using ERP.Core.Swagger.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.Core.Swagger.Filters;

public class SwaggerResponseTypesSetterFilter: IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadataAttribute = context.MethodInfo.GetCustomAttributes(true)
            .OfType<ApiOperationMetadataAttribute>()
            .FirstOrDefault();

        if (metadataAttribute != null)
        {
            // Get the Type of your base attribute class: ApiOperationMetadataAttribute
            var baseAttributeType = typeof(ApiOperationMetadataAttribute);

            // Get all ProducesResponseTypeAttribute instances applied to that class
            var responseAttributes = baseAttributeType.GetCustomAttributes<ProducesResponseTypeAttribute>(inherit: true);

            foreach (var responseAttr in responseAttributes)
            {
                var statusCode = responseAttr.StatusCode.ToString();

                // Only process if the status code isn't already handled by the method itself
                if (!operation.Responses.ContainsKey(statusCode))
                {
                    var response = new OpenApiResponse { Description = GetDefaultDescription(responseAttr.StatusCode) };

                    // If the response type is NOT void, we need to generate its schema
                    if (responseAttr.Type != typeof(void))
                    {
                        var errorSchema = context.SchemaGenerator.GenerateSchema(responseAttr.Type, context.SchemaRepository);
                        response.Content.Add("application/json", new OpenApiMediaType { Schema = errorSchema });
                    }
                    
                    // Add the response to the operation
                    operation.Responses.Add(statusCode, response);
                }
            }
        }
    }
    
    private string GetDefaultDescription(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "Error"
    };
}