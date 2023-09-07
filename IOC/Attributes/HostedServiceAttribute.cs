using Microsoft.Extensions.DependencyInjection;
using Reformat.Framework.Core.IOC.Core;

namespace Reformat.Framework.Core.IOC.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class HostedServiceAttribute : BaseServiceAttribute
{
    public HostedServiceAttribute(params Type[] registerAs) : base(registerAs)
    {
    }
}