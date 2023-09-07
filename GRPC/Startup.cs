using System.Net;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reformat.Framework.Core.GRPC;

/// <summary>
/// 启动配置
/// </summary>
public static class Startup
{
    public const string HTTP2_SETTING = GlobalStartup.ROOT_SETTING + "HTTP2_PORT";
    private static string DEFAULT_HTTP2_PORT = "18080";

    public static void AddGrpcSupport(this WebApplicationBuilder builder)
    {
        // 启动配置
        IConfiguration cfg = builder.Configuration;
        string? http2 = cfg.GetValue(HTTP2_SETTING,DEFAULT_HTTP2_PORT);
        builder.WebHost.ConfigureKestrel(options =>
        {

            // 配置 HTTP/2
            options.Listen(IPAddress.Any, Convert.ToInt32(http2), listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
        
        //开启服务
        builder.Services.AddGrpc();
        
        // builder.Services.AddSingleton<GreeterService>();
        
        // builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
        // {
        //     builder.AllowAnyOrigin()
        //         .AllowAnyMethod()
        //         .AllowAnyHeader()
        //         .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        // }));
    }
    
    public static void AddGrpcClient<T>(this WebApplicationBuilder builder,string requestUrl) where T : ClientBase
    {
        builder.Services.AddGrpcClient<T>(o =>
        {
            o.Address = new Uri(requestUrl);
        });
    }

    public static void AddGrpcService<T>(this WebApplication app) where T : class
    {
        app.UseEndpoints(endpoints => endpoints.MapGrpcService<T>());
    }
}
