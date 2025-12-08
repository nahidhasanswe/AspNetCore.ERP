using ERP.Api.Examples.Test;
using ERP.Core.Mapping;
using ERP.Core.Swagger.Attributes;
using ERP.Core.Web.Controller;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Api;

[ApiController]
[Route("api/test")]
public class TestApiController(ILogger logger, IObjectMapper mapper) : ApiControllerBase(logger, mapper)
{
    [HttpGet]
    [ApiReadResponse(typeof(TestModel), typeof(TestModelExample))]
    public IActionResult GetExampleData()
    {
        return Ok();
    }
}