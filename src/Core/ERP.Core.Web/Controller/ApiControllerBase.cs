using ERP.Core.Web.Response;
using ERP.Core.Collections;
using ERP.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ERP.Core.Web.Controller;

public class ApiControllerBase(ILogger logger, IObjectMapper? mapper = null) : ControllerBase()
{
    private ILogger Logger => logger;
    private IObjectMapper Mapper => mapper ?? throw new NullReferenceException("Mapper is null");

    protected internal PagedOkResult<T> PagedOk<T>(IPagedList<T> content)
        where T : class
    {
        return new PagedOkResult<T>(content);
    }

    protected OkResult<T> OkResult<T>(T content)
        where T : class
    {
        return new OkResult<T>(content, "Success");
    }

    protected internal OkResult<T> OkResult<T>(T content, string message)
        where T : class
    { 
        return new OkResult<T>(content, message);
    }

    protected IActionResult GetResult<T>(Result<T> result)
        where T: class
    {
        if (result.IsFailure)
        {
            return BadRequestResponse(result.Error);
        }
        
        return OkResult(result.Value);
    }

    protected async Task<IActionResult> GetResult<T>(Result<T> result, Func<Task<object>> factory)
    {
        return result.IsSuccess ? OkResult(await factory()) : BadRequestResponse(result.Error);
    }
    
    protected async Task<IActionResult> GetResultAsAction<T>(Result<T> result, Func<Task<IActionResult>> factory)
    {
        return result.IsSuccess ? await factory(): BadRequestResponse(result.Error);
    }

    protected IActionResult GetResultAsNotFoundIfFailed<T>(Result<T> result)
        where T : class
    {
        return result.IsSuccess ? OkResult(result.Value) : NotFoundResponse(result.Error);
    }

    protected internal IActionResult BadRequestResponse(string message, IDictionary<string, string[]> errors = null)
    {
        return BadRequest(new ApiResponse(message, System.Net.HttpStatusCode.BadRequest, errors));
    }

    protected internal IActionResult NotFoundResponse(string message)
    {
        return NotFound(new ApiResponse(message, System.Net.HttpStatusCode.NotFound));
    }
}