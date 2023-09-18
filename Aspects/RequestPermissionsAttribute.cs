using System.Reflection;
using AspectInjector.Broker;
using Reformat.Framework.Core.Common.Extensions.lang;
using Reformat.Framework.Core.Exceptions;
using Reformat.Framework.Core.JWT.interfaces;

namespace Reformat.Framework.Core.Aspects;

/// <summary>
/// 权限接口：需要自定义实现切面逻辑
/// </summary>
[AttributeUsage(AttributeTargets.Class  | AttributeTargets.Method)]
[Injection(typeof(RequestPermissionsAspect))]
public class RequestPermissionsAttribute : Attribute
{
    public string PowerCode { get; set; }

    public string[] IgnoreMethodName { get; set; }

    public RequestPermissionsAttribute(string powerCode, params string[] ignoreMethodName)
    {
        PowerCode = powerCode;
        IgnoreMethodName = ignoreMethodName;
    }
}


[Aspect(Scope.Global)]
public class RequestPermissionsAspect : Attribute
{
    [Advice(Kind.Before, Targets = Target.Method | Target.AnyAccess)]
    public void Before(
        [Argument(Source.Instance)] object Instance,
        [Argument(Source.Metadata)] MethodBase Metadata,
        [Argument(Source.Type)] Type Type,
        [Argument(Source.Triggers)] Attribute[] Triggers,
        [Argument(Source.Name)] string name)
    {
        RequestPermissionsAttribute? permissions = null;
        foreach (var trigger in Triggers)
        {
            if (trigger is RequestPermissionsAttribute)
            {
                permissions = (RequestPermissionsAttribute)trigger;
            }
        }
        if (permissions == null) return;
        IUserSupport obj = Instance as IUserSupport;
        if (obj == null) throw new Exception("目标类未继承 IUserSupport");

        if (permissions.IgnoreMethodName != null && permissions.IgnoreMethodName.Contains(name)) return;
        
        if (permissions.PowerCode.IsNullOrEmpty())
        {
            permissions.PowerCode = Instance.GetType().Name + ":" + name;
        }

        if (!obj.UserService.CheckPermissions(permissions.PowerCode))
        {
            throw new PermissionException("无权限操作");
        }
    }
}