using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Reformat.Framework.Core.MVC;

namespace Reformat.Framework.Core.Exceptions.Filter;

/// <summary>
/// 全局异常处理
/// </summary>
public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public Task OnExceptionAsync(ExceptionContext context)
    {
        // 如果异常没有被处理则进行处理
        if (context.ExceptionHandled == false)
        {
            Microsoft.AspNetCore.Http.IFormCollection formData = null;

            try
            {
                formData = context.HttpContext.Request.Form;
            }
            catch { }
            
            context.ExceptionHandled = true;
            context.Result = new JsonResult(new ApiResult(false,500, context.Exception.Message, default));
        }
        return Task.CompletedTask;
    }
}