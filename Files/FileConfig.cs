using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Reformat.Framework.Core.Files;

public static class FileConfig
{
    public static readonly string ROOT_PATH = AppDomain.CurrentDomain.BaseDirectory;
    public static string WEB_ROOT_PATH = ROOT_PATH;
    
    /// <summary>
    /// 静态文件配置
    /// Warinning: 文件预览还需要增加 builder.Services.AddDirectoryBrowser();
    /// </summary>
    /// <param name="app"></param>
    /// <param name="wwwroot"></param>
    /// <param name="enableBrowse"></param>
    public static void ExposeStaticFile(this WebApplication app,bool wwwroot = true,bool enableBrowse = false)
    {
        WEB_ROOT_PATH = app.Environment.ContentRootPath;
        
        Console.WriteLine("");
        Console.WriteLine("*****************静态文件配置 开始********************");
        
        IConfiguration cfg = app.Configuration;
        
        if (wwwroot)
        {
            app.UseFileServer(enableDirectoryBrowsing: enableBrowse);
            Console.WriteLine($"已启用默认静态目录暴露：wwwroot   文件预览:{enableBrowse}");
        }
        
        // 上传目录配置 Demo:
        // "upload": {
        //     "file": "upload",
        //     "fileSizeLimit": 10,
        //     "allPath": true,
        //     "host": ""
        // }
        var uploadPath = cfg.GetSection("upload:file").Value;
        if (uploadPath != null)
        {
            string filePath = Path.Combine(WEB_ROOT_PATH, uploadPath);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(filePath),
                RequestPath = uploadPath,
                EnableDirectoryBrowsing = enableBrowse,
            });
            Console.WriteLine($"完成上传文件目录暴露:{filePath}    访问地址:{uploadPath}   文件预览:{enableBrowse}");
        }
        
        // 20230805
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     FileProvider = new PhysicalFileProvider(Path.Combine(FILE_ROOT_PATH, filePath)),
        //     RequestPath = requestPath,
        //     ServeUnknownFileTypes = true,
        //     // DefaultContentType = "image/jpeg"
        // });
        
        Console.WriteLine("*****************静态文件配置 完成********************");
        Console.WriteLine("");
    }
}