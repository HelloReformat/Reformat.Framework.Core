namespace Reformat.Framework.Core.MVC;

/// <summary>
/// API开关注解
/// 拦截器详见：ApiExposedFilter
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ApiSwitchAttribute : Attribute
{
    /// <summary>
    /// 全局开关
    /// Warning：优先级最高
    /// </summary>
    public bool Enable { get; set; } = true;
    
    /// <summary>
    /// 仅开发环境开启
    /// Warning：优先级最高
    /// </summary>
    public bool OnlyDevEnv { get; set; } = false;
    
    /// <summary>
    /// 默认实现开关
    /// Warning：优先级中等
    /// </summary>
    public bool EnableDefaultApi { get; set; } = true;
    /// <summary>
    /// 自定义实现开关
    /// Warning：优先级中等
    /// </summary>
    public bool EnableCustomApi { get; set; } = true;
    
    /// <summary>
    /// BASEController<T> 指定暴露的接口
    /// Warning：优先级最低
    /// </summary>
    public AluApiTargets AluExposed { get; set; } = AluApiTargets.All;
    
    /// <summary>
    /// BaseController<T,TService> 指定暴露的接口
    /// Warning：优先级最低
    /// </summary>
    public DefaultApiTargets DefaultExposed { get; set; } = DefaultApiTargets.All;
    
    /// <summary>
    /// 额外扩展的方法 指定暴露的接口（方法名称匹配）
    /// Warning：优先级最低
    /// </summary>
    public string[] CustomExposed { get; set; }
}

/// <summary>
/// BASEController<T> 指定暴露的接口  
/// 与方法名称匹配
/// </summary>
public enum AluApiTargets
{
    GetModel = 1,
    Saveable = 2,
    add = 4,
    addList = 8,
    update = 16,
    updateStep = 32,
    select = 64,
    Export = 128,
    delete = 256,
    drop = 512,
    Import = 1024,
    DoAction = 2048,
    All = GetModel | Saveable | add | addList | update | updateStep| select| Export| delete| drop| Import| DoAction
}

/// <summary>
/// BaseController<T,TService> 中的方法
/// 与方法名称匹配
/// </summary>
public enum DefaultApiTargets
{
    Entity = 1,
    List = 2,
    Page = 4,
    Create = 8,
    Update = 16,
    Detele = 32,
    All = Entity | List | Page | Create | Update | Detele
}