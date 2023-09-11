using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.JWT.interfaces;

namespace Reformat.Framework.Core.JWT;

/// <summary>
/// 启动配置
/// </summary>
public static class Startup
{
    public static void AddUserService<T>(this WebApplicationBuilder builder) where T : class, IUserService
    {
        builder.Services.AddScoped<IUserService,T>();
    }
}
