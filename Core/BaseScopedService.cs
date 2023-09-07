using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;

namespace Reformat.Framework.Core.Core;

/// <summary>
/// 实现类需加上：[ScopedService]
/// </summary>
public abstract class BaseScopedService
{
    protected IocScoped iocService { get; set; }
    
    public BaseScopedService(IocScoped iocScoped)
    {
        iocScoped.Autowired(this);
        this.iocService = iocScoped;
    }
}