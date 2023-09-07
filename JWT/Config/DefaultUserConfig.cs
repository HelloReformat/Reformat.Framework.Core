using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;
using Reformat.Framework.Core.JWT.Core;
using Reformat.Framework.Core.JWT.interfaces;

namespace Reformat.Framework.Core.JWT.Config;

[SingleService(typeof(IUserService))]
public class DefaultUserConfig : BaseUserConfig
{
    public DefaultUserConfig(IocSingle iocService) : base(iocService)
    {
    }

    public override IUser GetFromRedis(string token)
    {
        throw new NotImplementedException("请继承IUserConfig并实现相关方法,并通过builder.Services.AddUserSupport<XXX>();进行注册");
    }

    public override Task<List<IUser>> GetUserByIds(long[] ids = null)
    {
        throw new NotImplementedException("请继承IUserConfig并实现相关方法,并通过builder.Services.AddUserSupport<XXX>();进行注册");
    }
}