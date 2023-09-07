using FreeRedis;
using Microsoft.Extensions.Configuration;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Services;
using Reformat.Framework.Core.JWT.Domain;
using Reformat.Framework.Core.JWT.interfaces;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.JWT.Core;

public abstract class BaseUserConfig : IUserService
{
    [Autowired] protected IConfiguration Configuration;
    
    protected readonly RedisClient redis;

    public abstract IUser GetFromRedis(string token);

    public BaseUserConfig(IocSingle iocService)
    {
        iocService.Autowired(this);
        
        string config = IocSingle.GetConfiguration().GetSection("redis").Value;
        if (config != null)
        {
            redis = new RedisClient(config);
            if (redis.Ping() != "PONG") throw new Exception("Redis连接失败");
        }
        //if (config == null) throw new Exception("未完成Redis配置");
    }
    
    #region UserInfo

    public async Task<IUser> GetUserById(long id)
    {
#if DEBUG
        if (DebugIds.Contains(id))return DebugUser.GetDebugUser();
#endif
        
        List<IUser> users = await GetUserByIds(new[] { id });
        if (users == null || users.Count == 0) {throw new Exception("账号异常：查询不到用户信息");}
        if (users.Count > 1) throw new Exception("账号异常：当前ID存在多个用户");
        return users[0];
    }

    /// <summary>
    /// 获取用户列表，必须实现
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public abstract Task<List<IUser>> GetUserByIds(long[] ids = null);

    // public abstract Task<List<long>> GetAllUserIds();

    #endregion


    #region Token

    public IUser GetUserByToken(string token)
    {
#if DEBUG
        if (DebugTokens.Contains(token))return DebugUser.GetDebugUser();
#endif
        if(token.IsNullOrEmpty())throw new Exception("非法请求：token不能为空");
        IUser user = GetFromRedis(token);
        if (user == null)
        {
            throw new Exception("非法请求：用户尚未登录");
        }
        return user != null ? user : null;
    }

    #endregion
    
    
    # region DebugUser
    protected List<long> DebugIds = new List<long>() {-1};
    protected List<string> DebugTokens = new List<string>() {"dev","cqx"};

    # endregion

}