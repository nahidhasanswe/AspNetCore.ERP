namespace ERP.Core.Swagger.Filters;

/// <summary>
/// A dedicated operation filter to remove the CancellationToken parameter 
/// from the Swagger documentation, as it is injected by ASP.NET Core and not 
/// provided by the client. This uses the parameter's TYPE for robustness.
/// </summary>
public class CancellationTokenFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        // Get all method parameters from the reflected MethodInfo
        var methodParameters = context.MethodInfo.GetParameters();

        // Identify the CancellationToken parameter type
        Type cancellationTokenType = typeof(CancellationToken);

        // Find the method parameters that are of type CancellationToken
        var tokenParameters = methodParameters
            .Where(p => p.ParameterType == cancellationTokenType)
            .ToList();

        // If CancellationToken parameters exist in the method signature, remove them 
        // from the generated OpenAPI 'Parameters' list.
        if (tokenParameters.Any())
        {
            // The OpenAPI parameters list contains entries by name.
            foreach (var tokenParam in tokenParameters)
            {
                var parameterToRemove = operation.Parameters
                    .FirstOrDefault(p => p.Name.Equals(tokenParam.Name, StringComparison.OrdinalIgnoreCase));
                
                if (parameterToRemove != null)
                {
                    operation.Parameters.Remove(parameterToRemove);
                }
            }
        }
    }
}