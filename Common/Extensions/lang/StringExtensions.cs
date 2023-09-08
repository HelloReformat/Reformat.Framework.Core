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
    
    /// <summary>忽略大小写的字符串结束比较，判断是否以任意一个待比较字符串结束</summary>
    /// <param name="value">字符串</param>
    /// <param name="strs">待比较字符串数组</param>
    /// <returns></returns>
    public static Boolean EndsWithIgnoreCase(this String? value, params String[] strs)
    {
        if (value == null || String.IsNullOrEmpty(value)) return false;

        foreach (var item in strs)
        {
            if (value.EndsWith(item, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}