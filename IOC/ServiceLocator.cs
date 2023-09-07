using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.IOC.Exceptions;

namespace Reformat.Framework.Core.IOC;

/// <summary>
/// 只能拿到 单例和瞬态
/// !!! 需要在启动时进行注册
/// </summary>
public static class ServiceLocator
{
    public static IServiceProvider Instance { get; private set; }
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        Instance = serviceProvider;
    }

    public static T GetService<T>()
    {
        if (Instance == null) throw new IocRegisterException("尚未注册ServiceLocator");
        return Instance.GetService<T>() ?? throw new IocException("尚未找到相关Bean+"+ typeof(T)); 
    }
    
    public static IEnumerable<T> GetServices<T>()
    {
        if (Instance == null) throw new IocRegisterException("尚未注册ServiceLocator");
        return Instance.GetServices<T>().Count() == 0 ? Instance.GetServices<T>() : throw new IocException("尚未找到相关Bean+"+ typeof(T));
    }
}