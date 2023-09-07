using System.Reflection;
using AspectInjector.Broker;

namespace Reformat.Framework.Core.Aspects;

/// <summary>
/// 配置显示
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[Injection(typeof(ConfigDisplayAspect))]
public class ConfigDisplayAttribute : Attribute
{
    public string ConfigName { get; set; }
    
    public string ConfigStart { get; set; }
    
    public string ConfigAfter { get; set; }

    public ConfigDisplayAttribute(string ConfigName,string ConfigStart = "",string ConfigAfter = "")
    {
        this.ConfigName = ConfigName;
        this.ConfigStart = ConfigStart;
        this.ConfigAfter = ConfigAfter;
    }
}

[Aspect(Scope.Global)]
public class ConfigDisplayAspect
{
    [Advice(Kind.Around, Targets = Target.Method)]
    public object Around(
        [Argument(Source.Metadata)] MethodBase Metadata,
        [Argument(Source.Target)] Func<object[], object> target,
        [Argument(Source.Arguments)] object[] args)
    {
        ConfigDisplayAttribute? displayAttribute = Metadata.GetCustomAttribute<ConfigDisplayAttribute>();
        Console.WriteLine("");
        Console.WriteLine($"*****************{displayAttribute.ConfigName} 开始********************");
        Console.WriteLine(displayAttribute.ConfigStart);
        object result = target(args);
        Console.WriteLine(displayAttribute.ConfigAfter);
        Console.WriteLine($"*****************{displayAttribute.ConfigName} 完成********************");
        Console.WriteLine("");
        Console.WriteLine("");
        return result;
    }
}


