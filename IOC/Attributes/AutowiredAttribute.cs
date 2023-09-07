namespace Reformat.Framework.Core.IOC.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AutowiredAttribute : Attribute
{
    /// <summary>
    /// 指定标识类
    /// </summary>
    public Type Identifier { get; set; }

    public bool IsServicesCall { get; set; } = false;
    
    public AutowiredAttribute(){}
    public AutowiredAttribute(Type identifier) => Identifier = identifier;
}