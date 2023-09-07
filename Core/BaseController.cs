using Microsoft.AspNetCore.Mvc;
using Reformat.Framework.Core.Aspects;
using Reformat.Framework.Core.IOC.Services;

namespace Reformat.Framework.Core.Core;

[ApiController]
[Route("api/[controller]/[action]")]
[ExceptionHandle]
public abstract class BaseController : ControllerBase
{
    public IocScoped iocScoped;
    
    public BaseController(IocScoped iocScoped)
    {
        iocScoped.Autowired(this);
        this.iocScoped = iocScoped;
    }
}