using System.Security.Cryptography;
using System.Text;

namespace Reformat.Framework.Core.Common.Extensions.lang;

public static class StringExtensions
{
    public static string GetMD5(this string input)
    {
        using (var md5 = MD5.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
    
    public static Boolean IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    
    public static Boolean NotIsNullOrEmpty(this String? value)=> !value.IsNullOrEmpty();
    
    public static string ToStringMissNull(this object str, string defvalue = "") => str != null ? str.ToString() : defvalue;
    
    /// <summary>
    /// 转小驼峰
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}