using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.IOC.Core;

namespace Reformat.Framework.Core.IOC.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class ScopedServiceAttribute  : BaseServiceAttribute
{
    public ScopedServiceAttribute(params Type[] registerAs) : base(registerAs)
    {
        this.Lifetime = ServiceLifetime.Scoped;
    }
}