using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.Files;

public static class FileUtils
{
    static FileUtils()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    
    /// <summary>
    /// 程序根目录
    /// </summary>
    /// <returns></returns>
    public static string GetApplicationDirectory() => FileConfig.WEB_ROOT_PATH;

    /// <summary>
    /// 项目根目录
    /// </summary>
    /// <returns></returns>
    public static string GetDevelopmentDirectory() => Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

    /// <summary>
    /// 获取wwwroot目录
    /// </summary>
    /// <returns></returns>
    public static string GetWwwRootDirectory() => Path.Combine(GetApplicationDirectory(), "wwwroot");
    
    
    public static string GetResourceDirectoty() => Path.Combine(GetApplicationDirectory(), "Resource");
    
    /// <summary>
    /// 拼接路径
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string AddPath(this string dir, string path) => Path.Combine(dir, path);

    /// <summary>
    /// 获取文件MD5码
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileMD5(string path)
    {
        FileInfo file = new FileInfo(path);
        using (var md5 = MD5.Create())
        {
            using (var stream = file.OpenRead())
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

    /// <summary>
    /// 获取文件MD5码
    /// </summary>
    /// <param name="filePaths"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetFilesMD5(string[] filePaths)
    {
        Dictionary<string, string> md5Dict = new Dictionary<string, string>();
        foreach (string path in filePaths)
        {
            FileInfo jsonFile = new FileInfo(path);
            if (jsonFile.Length == 0) continue;
            using (var md5 = MD5.Create())
            {
                using (var stream = jsonFile.OpenRead())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    string md5String = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    md5Dict.Add(md5String, path);
                    Console.WriteLine($"{jsonFile.Name}: {md5String}");
                }
            }
        }
        return md5Dict;
    }


    /// <summary>
    /// JSON 转 实体
    /// </summary>
    /// <param name="json"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? ConvertJsonToObject<T>(string json) => JsonConvert.DeserializeObject<T>(json);

    /// <summary>
    /// 从JSON文件获取JSON字符串
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="node"></param>
    /// <param name="encodeType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string GetJsonFromFile(string filePath, string node = "", string encodeType = "UTF8")
    {
        Encoding encoding = Encoding.GetEncoding(encodeType);

        // 判断文件是否为 JSON 文件
        if (!IsJsonFile(filePath)) throw new Exception(filePath + " : " + "非Json文件");
        string jsonString = "";

        // 解析 JSON 文件
        using (StreamReader reader = new StreamReader(filePath, encoding))
        {
            jsonString = reader.ReadToEnd();
            // Console.WriteLine(jsonString);
        }

        JObject jsonObject = JObject.Parse(jsonString);

        if (node.IsNullOrEmpty()) return jsonString;
        return jsonObject[node] != null ? jsonObject[node].ToString() : "";
    }

    /// <summary>
    /// 判断是否是JSON文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsJsonFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        return string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase);
    }
}