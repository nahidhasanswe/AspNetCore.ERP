using ERP.Core.Web.Response;
using Swashbuckle.AspNetCore.Filters;

namespace ERP.Api.Examples;

public interface ICustomExampleProvider<TModel> : IExamplesProvider<ApiResponse<TModel>>
    where TModel : class
{
    ApiResponse<TModel> GetResponse(TModel model);
}