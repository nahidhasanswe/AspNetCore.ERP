using ERP.Core.Collections;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Core.Web.Response
{
    public class PagedOkResult<T> : OkObjectResult
       where T : class
    {
        public int TotalCount { get; }

        public PagedOkResult(IPagedList<T> content)
            : base(new ApiResponse(content?.Items?.ToArray(), content?.TotalCount ?? 0, "Success"))
        {
            
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            return base.ExecuteResultAsync(context);
        }        
    }
}
