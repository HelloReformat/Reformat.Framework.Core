using System.Text.RegularExpressions;

namespace Reformat.Framework.Core.Validator;

/// <summary>
/// 操作正则表达式的公共类
/// 使用方式：[RegularExpression(RegexHelper._Email, ErrorMessage = "RegularExpression: {0}格式非法")]
/// Regex 用法参考：https://learn.microsoft.com/zh-cn/dotnet/api/system.text.regularexpressions.regex.-ctor?redirectedfrom=MSDN&view=net-7.0
/// </summary>   
public class RegexHelper
{
    #region 常用正则验证模式字符串
    public enum ValidateType
    {
        Email,                 // 邮箱
        TelePhoneNumber,       // 固定电话（座机）
        MobilePhoneNumber,     // 移动电话
        Age,                   // 年龄（1-120 之间有效）
        Birthday,              // 出生日期
        Timespan,              // 时间戳
        IdentityCardNumber,    // 身份证
        IpV4,                  // IPv4 地址
        IpV6,                  // IPV6 地址
        Domain,                // 域名
        English,               // 英文字母
        Chinese,               // 汉字
        MacAddress,            // MAC 地址
        Url,                   // URL 
    }

    private static readonly Dictionary<ValidateType, string> keyValuePairs = new Dictionary<ValidateType, string>
    {
       { ValidateType.Email, _Email },
       { ValidateType.TelePhoneNumber,_TelephoneNumber },  
       { ValidateType.MobilePhoneNumber,_MobilePhoneNumber }, 
       { ValidateType.Age,_Age }, 
       { ValidateType.Birthday,_Birthday }, 
       { ValidateType.Timespan,_Timespan }, 
       { ValidateType.IdentityCardNumber,_IdentityCardNumber }, 
       { ValidateType.IpV4,_IpV4 }, 
       { ValidateType.IpV6,_IpV6 }, 
       { ValidateType.Domain,_Domain }, 
       { ValidateType.English,_English }, 
       { ValidateType.Chinese,_Chinese }, 
       { ValidateType.MacAddress,_MacAddress }, 
       { ValidateType.Url,_Url }, 
    };

    public const string _Email = @"^(\w)+(\.\w)*@(\w)+((\.\w+)+)$"; // ^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$ , [A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}
    public const string _TelephoneNumber = @"(d+-)?(d{4}-?d{7}|d{3}-?d{8}|^d{7,8})(-d+)?"; //座机号码（中国大陆）
    public const string _MobilePhoneNumber = @"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$"; //移动电话
    public const string _Age = @"^(?:[1-9][0-9]?|1[01][0-9]|120)$"; // 年龄 1-120 之间有效
    public const string _Birthday = @"^((?:19[2-9]\d{1})|(?:20(?:(?:0[0-9])|(?:1[0-8]))))((?:0?[1-9])|(?:1[0-2]))((?:0?[1-9])|(?:[1-2][0-9])|30|31)$";
    public const string _Timespan = @"^15|16|17\d{8,11}$"; // 目前时间戳是15开头，以后16、17等开头，长度 10 位是秒级时间戳的正则，13 位时间戳是到毫秒级的。
    public const string _IdentityCardNumber = @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$";
    public const string _IpV4 = @"^((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})(\.((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})){3}$";
    public const string _IpV6 = @"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$";
    public const string _Domain = @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?$";
    public const string _English = @"^[A-Za-z]+$";
    public const string _Chinese = @"^[\u4e00-\u9fa5]{0,}$";
    public const string _MacAddress = @"^([0-9A-F]{2})(-[0-9A-F]{2}){5}$";
    public const string _Url = @"^[a-zA-z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
    #endregion

    /// <summary>
    /// 获取验证模式字符串
    /// </summary>
    /// <param name="validateType"></param>
    /// <returns></returns>
    public static (bool hasPattern, string pattern) GetValidatePattern(ValidateType validateType) 
    {
        bool hasPattern = keyValuePairs.TryGetValue(validateType, out string? pattern);
        return (hasPattern, pattern ?? string.Empty);
    }

    #region 验证输入字符串是否与模式字符串匹配
    /// <summary>
    /// 验证输入字符串是否与模式字符串匹配
    /// </summary>
    /// <param name="input">输入的字符串</param>
    /// <param name="validateType">模式字符串类型</param>
    /// <param name="matchTimeout">超时间隔</param>
    /// <param name="options">筛选条件</param>
    /// <returns></returns>
    public static (bool isMatch, string info) IsMatch(string input, ValidateType validateType, TimeSpan matchTimeout, RegexOptions options = RegexOptions.None)
    {
        var (hasPattern, pattern) = GetValidatePattern(validateType);
        if (hasPattern && !string.IsNullOrWhiteSpace(pattern))
        {
            bool isMatch = IsMatch(input, pattern, matchTimeout, options);
            if (isMatch) return (true, "Format validation passed."); // 格式验证通过。
            else return (false, "Format validation failed."); // 格式验证未通过。
        }

        return (false, "Unknown ValidatePattern."); // 未知验证模式
    }

    /// <summary>
    /// 验证输入字符串是否与模式字符串匹配，匹配返回true
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <param name="pattern">模式字符串</param>    
    /// <returns></returns>
    public static bool IsMatch(string input, string pattern)
    {
        return IsMatch(input, pattern, TimeSpan.Zero, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// 验证输入字符串是否与模式字符串匹配，匹配返回true
    /// </summary>
    /// <param name="input">输入的字符串</param>
    /// <param name="pattern">模式字符串</param>
    /// <param name="matchTimeout">超时间隔</param>
    /// <param name="options">筛选条件</param>
    /// <returns></returns>
    public static bool IsMatch(string input, string pattern, TimeSpan matchTimeout, RegexOptions options = RegexOptions.None)
    {
        return Regex.IsMatch(input, pattern, options, matchTimeout);
    }
    #endregion
}