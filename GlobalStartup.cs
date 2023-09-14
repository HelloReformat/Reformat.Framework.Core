using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reformat.Framework.Core.Converter;
using Reformat.Framework.Core.Exceptions.Filter;
using Reformat.Framework.Core.Files;
using Reformat.Framework.Core.IOC.Extensions;
using Reformat.Framework.Core.MVC;
using Reformat.Framework.Core.Swagger;

namespace Reformat.Framework.Core;

public static class GlobalStartup
{
    public const string ROOT_SETTING = "Application:";
    private const string HTTP1_SETTING = ROOT_SETTING + "HTTP1_PORT";
    private static string DEFAULT_HTTP1_PORT = "8080";
    
    //配置环境的指定
    private const string PROFILES_ACTIVE = "PROFILES_ACTIVE";
    private const string DEFAULT_ENVIRONMENT = "Development";
    
    //版本配置
    private const string APPLICATION_AURTHOR = ROOT_SETTING + "Aurthor";
    private const string APPLICATION_VERSION = ROOT_SETTING + "Version";
    private const string DEFAULT_APPLICATION_AURTHOR = "Ozkoalas";
    private const string DEFAULT_APPLICATION_VERSION = "1.0.0";
    
    public static void AddBoostSupport(this WebApplicationBuilder builder)
    {
        // 启动配置
        IConfiguration cfg = builder.Configuration;
        string? http1 = cfg.GetValue(HTTP1_SETTING,DEFAULT_HTTP1_PORT);
        builder.WebHost.ConfigureKestrel(options =>
        {
            // 配置 HTTP/1.1
            options.Listen(IPAddress.Any, Convert.ToInt32(http1), listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });
        
        // 通用配置
        // builder.Services.AddControllers();
        
        builder.Services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new ApiSwitchFilter(builder.Configuration));
            options.Filters.Add(typeof(GlobalExceptionFilter));
        });
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            // 禁用默认模型验证过滤器
            options.SuppressModelStateInvalidFilter = true;
        });
        
        builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                //修改属性名称的序列化方式，首字母小写
                options.SerializerSettings.ContractResolver = new CamelCaseContractResolver(); 
                // options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 特定类型序列化
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonDateTimeConvert());
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonDateTimeNullableConverter());
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonDateOnlyConvert());
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonDateOnlyNullableConverter());
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonTimeOnlyConvert());
                options.SerializerSettings.Converters.Add(new NewtonsoftJsonTimeOnlyNullableConverter());
                
                // 解决前端精度丢失的问题
                options.SerializerSettings.Converters.Add(new LongConverter());
                
                //TODO： 20330910：忽略循环引用 待测速
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }
        ).ConfigureApiBehaviorOptions(o =>
        {
            o.SuppressModelStateInvalidFilter = false;
            o.InvalidModelStateResponseFactory = (context) =>
            {
                var error = context.ModelState;
                return new ObjectResult(new ApiResult<ModelStateDictionary>()
                {
                    ErrorCode = 400, Message = "对象转换出错", Data = error
                });
            };
        });
        
        //Swagger
        builder.AddSwaggerSupport();
        
        // GRPC
        // builder.AddGrpcSupport();
        
        // SSE
        // builder.Services.AddServerSentEvent();

        // IOC
        builder.Services.AddServicesByAttribute();
        
        // 文件预览
        // builder.Services.AddDirectoryBrowser();
    }


    public static void AddBoostSupport(this WebApplication app)
    {
        // CORS
        app.UseCors("cors");
        
        // 添加身份验证和授权中间件
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        
        // IOC 全局辅助
        app.UseServiceLocator();
        
        // 扩展编码集
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        
        // 生产环境隔离
        if (!app.Environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();
            app.UseDeveloperExceptionPage();
            app.UseSwaggerConfig(true);
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        // Endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                context.Response.Redirect("/swagger");
                await context.Response.CompleteAsync();
            });
        });
    }
    
    public static void AddBanner(this WebApplication app)
    {
        IConfiguration cfg = app.Configuration;

        string filePath = AppDomain.CurrentDomain.BaseDirectory.AddPath("Banner.txt");
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine())!= null)
                {
                    if (line.StartsWith("1"))
                    {
                        string Aurther = cfg.GetValue(APPLICATION_AURTHOR, DEFAULT_APPLICATION_AURTHOR);
                        string Version = cfg.GetValue(APPLICATION_VERSION, DEFAULT_APPLICATION_VERSION);
                        string StartTime = DateTime.Now.ToString();
                        line = string.Format(line, Aurther, Version, StartTime);
                    }
                    if (line.StartsWith("2"))
                    {
                        IHostEnvironment environment = app.Services.GetService<IHostEnvironment>();
                        line = string.Format(line, environment.ApplicationName, environment.EnvironmentName, environment.ContentRootPath);
                    }
                    Console.WriteLine(line);
                }
            }
        }
    }
}