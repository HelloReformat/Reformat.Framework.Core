using Microsoft.Extensions.DependencyInjection;

namespace Reformat.Framework.Core.IOC.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public abstract class BaseServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; set; }
    
    public BaseServiceAttribute(params Type[] registerAs)
    {
        RegisterAs = registerAs;
    }

    public IEnumerable<Type> RegisterAs { get; set; }
}