using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.IOC.Exceptions;
using Reformat.Framework.Core.JWT.Core;

namespace Reformat.Framework.Core.IOC.Services;

[ScopedService]
public class IocScoped
{
    public HttpContextManager HttpContextManager;
    
    private IServiceProvider ServiceProvider{ set ; get; }
    private IServiceScopeFactory ServiceScopeFactory { set ; get; }
    
    public IocScoped(IServiceProvider serviceProvider,IServiceScopeFactory serviceScopeFactory,HttpContextManager httpContextManager)
    {
        ServiceProvider = serviceProvider;
        ServiceScopeFactory = serviceScopeFactory;
        HttpContextManager = httpContextManager;
    }
    
    private ConcurrentDictionary<Type, Action<object, IServiceProvider>> autowiredActions =
        new ConcurrentDictionary<Type, Action<object, IServiceProvider>>();

    public T? GetService<T>() => ServiceProvider.GetService<T>();
    public IEnumerable<T> GetServices<T>() => ServiceProvider.GetServices<T>();
    //private Func<IServiceProvider,Type,Object> _fuc1;
    /// <summary>
    /// Controller 需要在构造函数注入时进行调用
    /// </summary>
    /// <param name="service"></param>
    public void Autowired(object service)
    {
        var serviceType = service.GetType();
        if (autowiredActions.TryGetValue(serviceType, out Action<object, IServiceProvider> act))
        {
            act(service, ServiceProvider);
        }
        else
        {
            //参数
            var objParam = Expression.Parameter(typeof(object), "obj");
            var spParam = Expression.Parameter(typeof(IServiceProvider), "sp");

            var obj = Expression.Convert(objParam, serviceType);
            var GetService =
                typeof(IocScoped).GetMethod("GetService", BindingFlags.Static | BindingFlags.NonPublic);
            List<Expression> setList = new List<Expression>();

            //字段赋值
            foreach (FieldInfo field in serviceType.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var autowiredAttr = field.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    if (autowiredAttr.IsServicesCall)
                    {
                        // Type fieldType = field.FieldType;
                        // Func<IServiceProvider, Type, object> getServiceFunc = (serviceProvider, type) =>
                        // {
                        //     return ServiceProvider.GetServices(field.FieldType).ToList()[0];
                        // };
                        // Action serviceFunc = () => field.SetValue(service, getServiceFunc(ServiceProvider, fieldType));
                        // var fucn = serviceFunc();
                        // field.SetValue(service, serviceFunc);
                    }
                    else
                    {
                        var fieldExp = Expression.Field(obj, field);
                        var createService = Expression.Call(null, GetService, spParam,
                            Expression.Constant(field.FieldType), Expression.Constant(autowiredAttr));
                        var setExp = Expression.Assign(fieldExp, Expression.Convert(createService, field.FieldType));
                        setList.Add(setExp);
                    }
                }
            }

            //属性赋值
            foreach (PropertyInfo property in serviceType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var autowiredAttr = property.GetCustomAttribute<AutowiredAttribute>();
                if (autowiredAttr != null)
                {
                    if (autowiredAttr.IsServicesCall)
                    {
                        
                    }
                    else
                    {
                        var propExp = Expression.Property(obj, property);
                        var createService = Expression.Call(null, GetService, spParam,
                            Expression.Constant(property.PropertyType), Expression.Constant(autowiredAttr));

                        var setExp = Expression.Assign(propExp,
                            Expression.Convert(createService, property.PropertyType));
                        setList.Add(setExp);
                    }
                }
            }

            var bodyExp = Expression.Block(setList);
            var setAction = Expression.Lambda<Action<object, IServiceProvider>>(bodyExp, objParam, spParam)
                .Compile();
            autowiredActions[serviceType] = setAction;
            setAction(service, ServiceProvider);
        }
    }

    /// <summary>
    /// 根据不同的Identifier获取不同的服务实现
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="serviceType"></param>
    /// <param name="autowiredAttribute"></param>
    /// <returns></returns>
    private static object GetService(IServiceProvider serviceProvider, Type serviceType, AutowiredAttribute autowiredAttribute)
    {
        var list = serviceProvider.GetServices(serviceType).ToList();
        if (list.Count == 0)
        {
            throw new IocAutowiredException(serviceType+ "注入失败：未能找到相关容器");
        }
        // 未指定
        if (autowiredAttribute.Identifier == null)
        {
            return list.Last();
        }
        // 指定
        foreach (var item in list)
        {
            if (autowiredAttribute.Identifier == item.GetType()) return item;
        }
        throw new IocAutowiredException(serviceType+ "注入失败：未能找到相关容器");
    }
}