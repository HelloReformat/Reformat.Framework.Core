using Reformat.Framework.Core.Core;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;
using Reformat.Framework.Core.JWT.interfaces;

namespace Reformat.Framework.Core.JWT.Config;

[SingleService(typeof(IUserService))]
public class DefaultUserConfig : BaseSingleService,IUserService
{
    protected List<long> DebugIds = new List<long>() {-1};
    protected List<string> DebugTokens = new List<string>() {"dev","cqx"};
    
    public DefaultUserConfig(IocSingle iocService) : base(iocService)
    {
    }

    public IUser GetCurrentUser()
    {
        throw new NotImplementedException("请继承IUserConfig并实现相关方法,并通过builder.Services.AddUserSupport<XXX>();进行注册");
    }
}