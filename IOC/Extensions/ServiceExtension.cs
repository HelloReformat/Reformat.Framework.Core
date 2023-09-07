using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Core;
using Reformat.Framework.Core.IOC.Exceptions;

namespace Reformat.Framework.Core.IOC.Extensions;

public static class ServiceExtension
{
    /// <summary>
    /// IOC 全局辅助
    /// Warning：启动项配置（Program.cs）
    /// </summary>
    /// <param name="app">WebApplication</param>
    public static void UseServiceLocator(this WebApplication app) => ServiceLocator.SetServiceProvider(app.Services);
    
    /// <summary>
    /// IOC Bean扫描：扫描带有有 【ServiceAttribute】及其衍生注解的类
    /// Warning：启动项配置（Program.cs）
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns></returns>
    /// <exception cref="IocException"></exception>
    public static IServiceCollection AddServicesByAttribute(this IServiceCollection services)
    {
        // 20230306:GetAssemblies只能获取已加载的程序集，可能不全
        //var assemblies = AppDomain.CurrentDomain.GetAssemblies(); 

        // 方式一
        var files = Directory.GetFiles(AppContext.BaseDirectory, "*.dll");
        var assemblies1 = files.Select(x => Assembly.LoadFrom(x));
        // 方式二
        Assembly[] assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
        
        var types = assemblies2
            .SelectMany(f => f.GetTypes())
            .Where(e => e.GetCustomAttribute<BaseServiceAttribute>(false) != null);

        foreach (var type in types)
        {
            var serviceAttribute = type.GetCustomAttribute<BaseServiceAttribute>(false);
            var actorTypes = serviceAttribute.RegisterAs;
            // 默认注册为当前类
            if (actorTypes.Count() == 0)
            {
                actorTypes.Append(type);
            }
            
            // 类型检查
            foreach (var actorType in actorTypes)
            {
                //type 是否为 actorType 的实现或子类或本身
                if (!actorType.IsAssignableFrom(type)) throw new IocException("ICO注解设置错误：" +  type);
            }
            
            // 注册
            if (serviceAttribute is ScopedServiceAttribute)
            {
                services.AddScoped(type);
                foreach (var actorType in actorTypes)
                {
                    services.AddScoped(actorType, (sCol) => sCol.GetService(type));
                }
            }
            else if (serviceAttribute is SingleServiceAttribute)
            {
                services.AddSingleton(type);
                foreach (var actorType in actorTypes)
                {
                    services.AddSingleton(actorType, (sCol) => sCol.GetService(type));
                }
            }
            else if (serviceAttribute is TransientServiceAttribute)
            {
                services.AddTransient(type);
                foreach (var actorType in actorTypes)
                {
                    services.AddTransient(actorType, (sCol) => sCol.GetService(type));
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        // **********************废除**************************
        // var serviceTypes = typeof(T).Assembly.GetTypes()
        //     .Where(a => a.IsClass)
        //     .Where(a => a.GetCustomAttribute<InjectionAttribute>() != null)//扫描注解
        //     .Where(a => !a.IsAbstract);
        // foreach (var item in serviceTypes)
        // {
        //     var injection = item.GetCustomAttribute<InjectionAttribute>();
        //     if (injection!.ServiceType == null)
        //     {
        //         services.Add(new ServiceDescriptor(item, item, injection.Lifetime));
        //     }
        //     else
        //     {
        //         services.Add(new ServiceDescriptor(injection!.ServiceType, item, injection.Lifetime));
        //     }
        // }
        return services;
    }
    
}