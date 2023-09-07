using Reformat.Framework.Core.IOC.Services;

namespace Reformat.Framework.Core.Core;

/// <summary>
/// 实现类需加上：[SingleService]
/// </summary>
public abstract class BaseSingleService
{
    protected IocSingle iocSingle { get; set; }
    
    public BaseSingleService(IocSingle iocSingle)
    {
        iocSingle.Autowired(this);
        this.iocSingle = iocSingle;
    }
}