using System.ComponentModel;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.Converter;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class NumberTypeConvert
{
    private static readonly DateTime _dt1970 = new(1970, 1, 1);
    private static readonly DateTimeOffset _dto1970 = new(new DateTime(1970, 1, 1));

    /// <summary>转为整数，转换失败时返回默认值。支持字符串、全角、字节数组（小端）、时间（Unix秒）</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual Int32 ToInt(Object value, Int32 defaultValue)
    {
        if (value is Int32 num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str)
        {
            // 拷贝而来的逗号分隔整数
            str = str.Replace(",", null);
            str = ToDBC(str).Trim();
            return str.IsNullOrEmpty() ? defaultValue : Int32.TryParse(str, out var n) ? n : defaultValue;
        }
        else if (value is IList<String> list)
        {
            if (list.Count == 0) return defaultValue;
            if (Int32.TryParse(list[0], out var n)) return n;
        }

        // 特殊处理时间，转Unix秒
        if (value is DateTime dt)
        {
            if (dt == DateTime.MinValue) return 0;
            if (dt == DateTime.MaxValue) return -1;

            //// 先转UTC时间再相减，以得到绝对时间差
            //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
            // 保存时间日期由Int32改为UInt32，原截止2038年的范围扩大到2106年
            var n = (dt - _dt1970).TotalSeconds;
            return n >= Int32.MaxValue ? throw new InvalidDataException("时间过大，数值超过Int32.MaxValue") : (Int32)n;
        }
        if (value is DateTimeOffset dto)
        {
            if (dto == DateTimeOffset.MinValue) return 0;

            //return (Int32)(dto - _dto1970).TotalSeconds;
            var n = (dto - _dto1970).TotalSeconds;
            return n >= Int32.MaxValue ? throw new InvalidDataException("时间过大，数值超过Int32.MaxValue") : (Int32)n;
        }

        if (value is Byte[] buf)
        {
            if (buf == null || buf.Length <= 0) return defaultValue;

            switch (buf.Length)
            {
                case 1:
                    return buf[0];
                case 2:
                    return BitConverter.ToInt16(buf, 0);
                case 3:
                    return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                case 4:
                    return BitConverter.ToInt32(buf, 0);
                default:
                    break;
            }
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch { return defaultValue; }
    }

    /// <summary>转为长整数。支持字符串、全角、字节数组（小端）、时间（Unix毫秒）</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual Int64 ToLong(Object value, Int64 defaultValue)
    {
        if (value is Int64 num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str)
        {
            // 拷贝而来的逗号分隔整数
            str = str.Replace(",", null);
            str = ToDBC(str).Trim();
            return str.IsNullOrEmpty() ? defaultValue : Int64.TryParse(str, out var n) ? n : defaultValue;
        }
        else if (value is IList<String> list)
        {
            if (list.Count == 0) return defaultValue;
            if (Int64.TryParse(list[0], out var n)) return n;
        }

        // 特殊处理时间，转Unix毫秒
        if (value is DateTime dt)
        {
            if (dt == DateTime.MinValue) return 0;

            //// 先转UTC时间再相减，以得到绝对时间差
            //return (Int32)(dt.ToUniversalTime() - _dt1970).TotalSeconds;
            return (Int64)(dt - _dt1970).TotalMilliseconds;
        }
        if (value is DateTimeOffset dto)
        {
            return dto == DateTimeOffset.MinValue ? 0 : (Int64)(dto - _dto1970).TotalMilliseconds;
        }

        if (value is Byte[] buf)
        {
            if (buf == null || buf.Length <= 0) return defaultValue;

            switch (buf.Length)
            {
                case 1:
                    return buf[0];
                case 2:
                    return BitConverter.ToInt16(buf, 0);
                case 3:
                    return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                case 4:
                    return BitConverter.ToInt32(buf, 0);
                case 8:
                    return BitConverter.ToInt64(buf, 0);
                default:
                    break;
            }
        }

        //暂时不做处理  先处理异常转换
        try
        {
            return Convert.ToInt64(value);
        }
        catch { return defaultValue; }
    }

    /// <summary>转为浮点数</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual Double ToDouble(Object value, Double defaultValue)
    {
        if (value is Double num) return Double.IsNaN(num) ? defaultValue : num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str)
        {
            str = ToDBC(str).Trim();
            return str.IsNullOrEmpty() ? defaultValue : Double.TryParse(str, out var n) ? n : defaultValue;
        }
        else if (value is IList<String> list)
        {
            if (list.Count == 0) return defaultValue;
            if (Double.TryParse(list[0], out var n)) return n;
        }

        if (value is Byte[] buf && buf.Length <= 8)
            return BitConverter.ToDouble(buf, 0);

        try
        {
            return Convert.ToDouble(value);
        }
        catch { return defaultValue; }
    }

    /// <summary>转为高精度浮点数</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual Decimal ToDecimal(Object value, Decimal defaultValue)
    {
        if (value is Decimal num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str)
        {
            str = ToDBC(str).Trim();
            return str.IsNullOrEmpty() ? defaultValue : Decimal.TryParse(str, out var n) ? n : defaultValue;
        }
        else if (value is IList<String> list)
        {
            if (list.Count == 0) return defaultValue;
            if (Decimal.TryParse(list[0], out var n)) return n;
        }

        if (value is Byte[] buf)
        {
            if (buf == null || buf.Length <= 0) return defaultValue;

            switch (buf.Length)
            {
                case 1:
                    return buf[0];
                case 2:
                    return BitConverter.ToInt16(buf, 0);
                case 3:
                    return BitConverter.ToInt32(new Byte[] { buf[0], buf[1], buf[2], 0 }, 0);
                case 4:
                    return BitConverter.ToInt32(buf, 0);
                default:
                    // 凑够8字节
                    if (buf.Length < 8)
                    {
                        var bts = new Byte[8];
                        Buffer.BlockCopy(buf, 0, bts, 0, buf.Length);
                        buf = bts;
                    }
                    // 20230314 : 补Null
                    return ((IConvertible)BitConverter.ToDouble(buf, 0)).ToDecimal(null);
            }
        }

        if (value is Double d)
        {
            return Double.IsNaN(d) ? defaultValue : (Decimal)d;
        }

        try
        {
            return Convert.ToDecimal(value);
        }
        catch { return defaultValue; }
    }

    /// <summary>转为布尔型。支持大小写True/False、0和非零</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual Boolean ToBoolean(Object value, Boolean defaultValue)
    {
        if (value is Boolean num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str)
        {
            //str = ToDBC(str).Trim();
            str = str.Trim();
            if (str.IsNullOrEmpty()) return defaultValue;

            if (Boolean.TryParse(str, out var b)) return b;

            if (String.Equals(str, Boolean.TrueString, StringComparison.OrdinalIgnoreCase)) return true;
            if (String.Equals(str, Boolean.FalseString, StringComparison.OrdinalIgnoreCase)) return false;

            // 特殊处理用数字0和1表示布尔型
            str = ToDBC(str);
            return Int32.TryParse(str, out var n) ? n > 0 : defaultValue;
        }
        else if (value is IList<String> list)
        {
            if (list.Count == 0) return defaultValue;
            if (Int32.TryParse(list[0], out var n)) return n > 0;
        }

        try
        {
            return Convert.ToBoolean(value);
        }
        catch { return defaultValue; }
    }

    

    /// <summary>全角为半角</summary>
    /// <remarks>全角半角的关系是相差0xFEE0</remarks>
    /// <param name="str"></param>
    /// <returns></returns>
    private static String ToDBC(String str)
    {
        var ch = str.ToCharArray();
        for (var i = 0; i < ch.Length; i++)
        {
            // 全角空格
            if (ch[i] == 0x3000)
                ch[i] = (Char)0x20;
            else if (ch[i] is > (Char)0xFF00 and < (Char)0xFF5F)
                ch[i] = (Char)(ch[i] - 0xFEE0);
        }
        return new String(ch);
    }

    

    /// <summary>字节单位字符串</summary>
    /// <param name="value">数值</param>
    /// <param name="format">格式化字符串</param>
    /// <returns></returns>
    public virtual String ToGMK(UInt64 value, String format = null)
    {
        if (value < 1024) return $"{value:n0}";

        if (format.IsNullOrEmpty()) format = "n2";

        var val = value / 1024d;
        if (val < 1024) return val.ToString(format) + "K";

        val /= 1024;
        if (val < 1024) return val.ToString(format) + "M";

        val /= 1024;
        if (val < 1024) return val.ToString(format) + "G";

        val /= 1024;
        if (val < 1024) return val.ToString(format) + "T";

        val /= 1024;
        if (val < 1024) return val.ToString(format) + "P";

        val /= 1024;
        return val.ToString(format) + "E";
    }
}