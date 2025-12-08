namespace ERP.Core.Swagger.Configuration;

public class SwaggerApiGenericResponseConfig(Type genericType)
{
    public Type GetType() => genericType;
}