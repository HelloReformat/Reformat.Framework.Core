using System.Reflection;
using AspectInjector.Broker;
using Microsoft.AspNetCore.Mvc.Routing;
using Reformat.Framework.Core.Exceptions;
using Reformat.Framework.Core.MVC;

namespace Reformat.Framework.Core.Aspects;

/// <summary>
/// 异常捕获
/// Warning：只能在Controller中使用，且返回类型不能是复合抽象（ApiResult<T>）,T不能是Object
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[Aspect(Scope.Global)]
[Injection(typeof(ExceptionHandleAttribute))]
public class ExceptionHandleAttribute : Attribute
{
    private static MethodInfo asyncErrorHandler =
        typeof(ExceptionHandleAttribute).GetMethod(nameof(WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);

    private static MethodInfo syncErrorHandler =
        typeof(ExceptionHandleAttribute).GetMethod(nameof(WrapSync), BindingFlags.NonPublic | BindingFlags.Static);

    [Advice(Kind.Around, Targets = Target.Method)]
    public object ExceptionHandle(
        [Argument(Source.Instance)] object Instance,
        [Argument(Source.Target)] Func<object[], object> target,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.ReturnType)] Type retType,
        [Argument(Source.Name)] string name,
        [Argument(Source.Triggers)] Attribute[] triggers,
        [Argument(Source.Metadata)] MethodBase Metadata)
    {
        if (!IsExistHttpAttribute(Metadata))
        {
            return target(args);
        }

        if (typeof(Task).IsAssignableFrom(retType))
        {
            var AsyncResultType = retType.IsConstructedGenericType ? retType.GenericTypeArguments[0] : typeof(object);
            AsyncResultType = GetAPIResponseContextType(name, AsyncResultType);
            return asyncErrorHandler.MakeGenericMethod(AsyncResultType).Invoke(this, new object[] { target, args });
        }
        else
        {
            var SyncResultType = GetAPIResponseContextType(name, retType);
            return syncErrorHandler.MakeGenericMethod(SyncResultType).Invoke(this, new object[] { target, args });
        }
    }

    /// <summary>
    /// 获取APIResponse<T>中的T
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Type GetAPIResponseContextType(string name, Type type)
    {
        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(ApiResult<>)))
        {
            type = type.GenericTypeArguments[0];
            if (type.FullName.Equals("System.Object"))
            {
                throw new Exception($"{name} : 根据孝哥的前后端约束，不能用Object作为抽象类型呢~");
            }
        }
        else
        {
            throw new Exception($"{name} : 不支持非APIResponse<T>类型的方法");
        }

        return type;
    }

    /// <summary>
    /// 判断是否有HTTP注解
    /// </summary>
    /// <param name="Metadata"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private bool IsExistHttpAttribute(MethodBase Metadata)
    {
        if (!Metadata.GetCustomAttributes().Any()) return false;

        foreach (var attribute in Metadata.GetCustomAttributes())
        {
            if (attribute is HttpMethodAttribute)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 同步方法
    /// </summary>
    /// <param name="target"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static ApiResult<T> WrapSync<T>(Func<object[], object> target, object[] args)
    {
        try
        {
            return (ApiResult<T>)target(args);
        }
        catch (PermissionException ex)
        {
            Console.WriteLine("权限不足: " + ex.Message);
            return Api.RestError<T>(ex.code, "权限不足", ex.message);
        }
        catch (BusinessException ex)
        {
            Console.WriteLine("业务异常: " + ex.message + ":" + ex.Message + "====>" + ex.StackTrace);
            return Api.RestError<T>(ex.code, "业务异常", ex.message + ex.details);
        }
        catch (ValidateException ex)
        {
            Console.WriteLine("验证不通过: " + ex.message + ":" + ex.Message + "====>" + ex.StackTrace);
            return Api.RestError<T>(ex.code, ex.message, ex.details != null ? ex.details.ToString() : "");
        }
        catch (Exception ex)
        {
            Console.WriteLine("系统异常: " + ex.Message + "====>" + ex.StackTrace);
            return Api.RestError<T>(500, "系统异常", ex.Message);
        }
    }

    /// <summary>
    /// 异步方法
    /// </summary>
    /// <param name="target"></param>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static async Task<ApiResult<T>> WrapAsync<T>(Func<object[], object> target, object[] args)
    {
        try
        {
            return await (Task<ApiResult<T>>)target(args);
        }
        catch (PermissionException ex)
        {
            Console.WriteLine("权限不足: " + ex.Message);
            return await Api.AsyncRestError<T>(ex.code, "权限不足", ex.message);
        }
        catch (BusinessException ex)
        {
            Console.WriteLine("业务异常: " + ex.message + ":" + ex.Message + "====>" + ex.StackTrace);
            return await Api.AsyncRestError<T>(ex.code, "业务异常", ex.message + ex.details);
        }
        catch (ValidateException ex)
        {
            Console.WriteLine("验证不通过: " + ex.message + ":" + ex.Message + "====>" + ex.StackTrace);
            return await Api.AsyncRestError<T>(ex.code, ex.message, ex.details != null ? ex.details.ToString() : "");
        }
        catch (Exception ex)
        {
            Console.WriteLine("系统异常: " + ex.Message + "====>" + ex.StackTrace);
            return await Api.AsyncRestError<T>(500, "系统异常", ex.Message);
        }
    }
}