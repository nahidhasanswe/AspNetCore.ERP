using System.Net;
using System.Reflection;
using System.Text.Json;
using ERP.Core.Swagger.Attributes;
using ERP.Core.Swagger.Configuration;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.Core.Swagger.Filters;

public class SwaggerTypeSetterFilter(SwaggerApiGenericResponseConfig config) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttribute<ApiOperationMetadataAttribute>();
        if (attribute == null) return;

        // 1. Handle Success Response (200 OK)
        var successCode = ((int)HttpStatusCode.OK).ToString();
        var responseMediaType = GetOrCreateMediaType(operation.Responses, successCode, $"Successful response containing a {attribute.ResponseType.Name} entity.");
        
        // Reverted: Use the actual ApiResponse generic type for schema generation
        var genericResponseType = attribute.ResponseType.IsGenericType ? config.GetType().MakeGenericType(attribute.ResponseType) : attribute.ResponseType;
        
        if (attribute.ResponseType.IsGenericType)
        
        responseMediaType.Schema = context.SchemaGenerator.GenerateSchema(genericResponseType, context.SchemaRepository);
        SetExampleFromProvider(responseMediaType, attribute.ExampleType, unwrapData: false);

        // 2. Handle Request Body (Only for Write attributes)
        if (attribute is ApiWriteResponseAttribute writeAttribute && writeAttribute.RequestType != null)
        {
            operation.RequestBody ??= new OpenApiRequestBody { Content = new Dictionary<string, OpenApiMediaType>() };
            var requestMediaType = GetOrCreateMediaType(operation.RequestBody.Content);

            requestMediaType.Schema = context.SchemaGenerator.GenerateSchema(writeAttribute.RequestType, context.SchemaRepository);
            SetExampleFromProvider(requestMediaType, writeAttribute.RequestExampleType, unwrapData: true);
        }
    }

    // Helper: Ensures the Dictionary and MediaType exist, returns the MediaType
    private OpenApiMediaType GetOrCreateMediaType(IDictionary<string, OpenApiResponse> responses, string statusCode, string description)
    {
        if (!responses.ContainsKey(statusCode)) responses[statusCode] = new OpenApiResponse();
        responses[statusCode].Description = description;
        return GetOrCreateMediaType(responses[statusCode].Content);
    }

    // Helper: Overload for Content Dictionaries
    private OpenApiMediaType GetOrCreateMediaType(IDictionary<string, OpenApiMediaType> content)
    {
        if (!content.ContainsKey("application/json")) content["application/json"] = new OpenApiMediaType();
        return content["application/json"];
    }

    // Helper: Generates and sets the example using Dynamic handling
    private void SetExampleFromProvider(OpenApiMediaType mediaType, Type exampleProviderType, bool unwrapData)
    {
        if (exampleProviderType == null) return;

        try
        {
            var provider = Activator.CreateInstance(exampleProviderType);
            var method = exampleProviderType.GetMethod("GetExamples");
            
            if (method != null)
            {
                var result = method.Invoke(provider, null);

                // Handle Dynamic Unwrapping of the "Data" property
                if (unwrapData && result != null)
                {
                    try 
                    {
                        dynamic dynamicResult = result;
                        // Try to access .Data dynamically, fallback to result if it fails
                        result = dynamicResult.Data; 
                    }
                    catch { /* Ignore if .Data doesn't exist */ }
                }

                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                
                mediaType.Examples.Clear();
                mediaType.Examples.Add("default", new OpenApiExample { Value = new OpenApiString(json) });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Swagger Filter Error ({exampleProviderType.Name}): {ex.Message}");
        }
    }
}
