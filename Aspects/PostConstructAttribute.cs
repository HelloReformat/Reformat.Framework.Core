using AspectInjector.Broker;
using Reformat.Framework.Core.Aspects.interfaces;

namespace Reformat.Framework.Core.Aspects;

/// <summary>
/// 用在构造函数上， 且类必须继承IPostConstruct接口 
/// </summary>
[AttributeUsage( AttributeTargets.Constructor)]
[Aspect(Scope.Global)]
[Injection(typeof(PostConstructAttribute))]
public class PostConstructAttribute : Attribute
{
    [Advice(Kind.After, Targets = Target.Constructor)]
    public void After([Argument(Source.Instance)] object Instance)
    {
        IPostConstruct obj = Instance as IPostConstruct;
        if (obj == null)
        {
            throw new Exception("目标类未继承 IPostConstruct");
        }
        obj.PostInit();
    }
}