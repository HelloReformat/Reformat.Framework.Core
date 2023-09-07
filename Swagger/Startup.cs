using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Reformat.Framework.Core.MVC;
using Reformat.Framework.Core.Swagger.Filter;

namespace Reformat.Framework.Core.Swagger;

/// <summary>
/// 启动配置
/// </summary>
public static class Startup
{
    public static void AddSwaggerSupport(this WebApplicationBuilder builder)
    {
        IConfiguration cfg = builder.Configuration;
        IServiceCollection services = builder.Services;

        // 20230622 : Controller注冊設置
        // services.AddControllers(options => { options.Conventions.Add(new ApiExposedFilter(cfg)); });
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var _name = AppDomain.CurrentDomain.FriendlyName;

        // 20230627
        // 获取目录下所有XML的文件路径
        List<string> xmlList = Directory.GetFiles(basePath, "*.xml", SearchOption.AllDirectories).ToList();

        services.AddSwaggerGen(option =>
        {
            // 20230425 : SwaggerSchema中新增显示自定义注解的设置
            // option.SchemaFilter<CustomSchemaFilter>();

            var securityScheme = new OpenApiSecurityScheme()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "token",
                //参数添加在头部
                In = ParameterLocation.Header,
                //使用Authorize头部
                Type = SecuritySchemeType.ApiKey,
                //内容为以 bearer开头
                Scheme = "",
                BearerFormat = "JWT"
            };

            //把所有方法配置为增加bearer头部信息
            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    },
                    new string[] { }
                }
            };

            option.SwaggerDoc("test", new OpenApiInfo()
            {
                Title = "API",
                Contact = new OpenApiContact() { Name = "ApiDoc" },
                Description = "项目文档",
                Version = "1.0"
            });

            // 20230627
            xmlList.ForEach(xmlPath =>
            {
                option.IncludeXmlComments(xmlPath, true);
            });

            option.SchemaFilter<SwaggerEnumDisplayFilter>(); // 枚举显示
            option.SchemaFilter<SwaggerIgnoreFilter>(); // 忽略指定字段和 属性 
            
            option.AddSecurityDefinition("bearerAuth", securityScheme);
            option.AddSecurityRequirement(securityRequirement);
            option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            option.CustomOperationIds(apiDesc =>
            {
                var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                return controllerAction.ControllerName + "：" + controllerAction.ActionName;
            });
        });
    }

    public static void UseSwaggerConfig(this WebApplication app, bool knife4UI = false)
    {
        // Swagger Setting
        app.UseSwagger();
        if (knife4UI)
        {
            app.UseKnife4UI(s => s.SwaggerEndpoint("/test/swagger.json", "api"));
        }
        else
        {
            app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/test/swagger.json", "api"); });
        }
    }
}