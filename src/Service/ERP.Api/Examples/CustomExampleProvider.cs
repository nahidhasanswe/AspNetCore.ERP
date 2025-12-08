using ERP.Core.Web.Response;

namespace ERP.Api.Examples;

public abstract class CustomExampleProvider<TModel> : ICustomExampleProvider<TModel>
    where TModel : class
{
    public abstract ApiResponse<TModel> GetExamples();

    public ApiResponse<TModel> GetResponse(TModel model)
    {
        return new ApiResponse<TModel>(model, "Success");
    }
}