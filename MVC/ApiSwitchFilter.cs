using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace Reformat.Framework.Core.MVC;

/// <summary>
/// API开关拦截器
/// </summary>
public class ApiSwitchFilter : IApplicationModelConvention
{
    private readonly bool isDevEnv;
    
    public ApiSwitchFilter(IConfiguration cfg)
    {
        string environmentName = cfg.GetSection("Environment")?.Value;
        if (environmentName == "Development")
        {
            isDevEnv = true;
        }
        else
        {
            isDevEnv = false;
        }
    }
    
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            // 筛选 带有ApiSwitch注解的 控制器
            var ApiSwitch = controller.ControllerType.GetCustomAttribute<ApiSwitchAttribute>();
            if (ApiSwitch == null)
            {
                continue;
            }

            IList<ActionModel> actions = controller.Actions;
            List<string> CustomActionNames = new List<string>();
            // 当前类定义的公有方法 （包含父级）
            // MethodInfo[] Methods = controller.ControllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            // 当前类定义的公有方法 （不包含父级）
            MethodInfo[] CustomdMethods = controller.ControllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            CustomdMethods.ToList().ForEach(i => CustomActionNames.Add(i.Name));
            
            actions.ToList().ForEach(action =>
            {
                bool result = false;
                
                if(!CustomActionNames.Contains(action.ActionName))
                {
                    // 内置接口
                    if (ApiSwitch.Enable)
                    {
                        // 仅开发环境约束判断
                        if((ApiSwitch.OnlyDevEnv && isDevEnv) || !ApiSwitch.OnlyDevEnv)
                        {
                            // 默认实现开关
                            if (ApiSwitch.EnableDefaultApi)
                            {
                                // 指定暴露的接口
                                if (Enum.IsDefined(typeof(DefaultApiTargets), action.ActionName))
                                {
                                    if (ApiSwitch.DefaultExposed.HasFlag(DefaultApiTargets.All))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        DefaultApiTargets apiTargets = (DefaultApiTargets)Enum.Parse(typeof(DefaultApiTargets), action.ActionName);
                                        if (ApiSwitch.DefaultExposed.HasFlag(apiTargets))
                                        {
                                            result = true;
                                        }
                                    }
                                }
                                
                                // 指定暴露的接口
                                if (Enum.IsDefined(typeof(AluApiTargets), action.ActionName))
                                {
                                    if (ApiSwitch.AluExposed.HasFlag(AluApiTargets.All))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        AluApiTargets apiTargets = (AluApiTargets)Enum.Parse(typeof(AluApiTargets), action.ActionName);
                                        if (ApiSwitch.AluExposed.HasFlag(apiTargets))
                                        {
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 自定义接口
                    if (ApiSwitch.Enable)
                    {
                        if ((ApiSwitch.OnlyDevEnv && isDevEnv) || !ApiSwitch.OnlyDevEnv)
                        {
                            if (ApiSwitch.EnableCustomApi)
                            {
                                if (ApiSwitch.CustomExposed == null || ApiSwitch.CustomExposed.Length == 0)
                                {
                                    result = true;
                                }
                                else if(ApiSwitch.CustomExposed.Contains(action.ActionName))
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
                switchExposed(ref action, result);
            });
        }
    }
    
    private void switchExposed(ref ActionModel action, bool exposed) => action.ApiExplorer.IsVisible = exposed;
    
    
    private void switchExposed(ref MethodInfo methodInfo, bool exposed)
    {
        var apiExplorerSettings = methodInfo.GetCustomAttributes<ApiExplorerSettingsAttribute>().FirstOrDefault();
        if (apiExplorerSettings != null)
        {
            apiExplorerSettings.IgnoreApi = !exposed;
        }
    }

    /// <summary>
    /// 判断是否有HTTP注解
    /// </summary>
    /// <param name="Metadata"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private bool IsExistHttpAttribute(MethodInfo methodInfo)
    {
        if (!methodInfo.GetCustomAttributes().Any()) return false;
    
        foreach (var attribute in methodInfo.GetCustomAttributes())
        {
            if (attribute is HttpMethodAttribute)
            {
                return true;
            }
        }
        return false;
    }
}