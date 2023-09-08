using System.Globalization;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.Converter;

/// <summary>
/// 缺少了timeolny dateoleny
/// </summary>
public class DateTypeConvert
{
    private static readonly Int64 _maxSeconds = (Int64)(DateTime.MaxValue - DateTime.MinValue).TotalSeconds;
    private static readonly Int64 _maxMilliseconds = (Int64)(DateTime.MaxValue - DateTime.MinValue).TotalMilliseconds;
    private static readonly DateTime _dt1970 = new(1970, 1, 1);
    private static readonly DateTimeOffset _dto1970 = new(new DateTime(1970, 1, 1));

    /// <summary>转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual DateTime ToDateTime(Object value, DateTime defaultValue)
    {
        if (value is DateTime num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str ||
            (value is IList<String> list && list.Count > 0 && (str = list[0]) != null))
        {
            //str = ToDBC(str).Trim();
            str = str.Trim();
            if (str.IsNullOrEmpty()) return defaultValue;

            // 处理UTC
            var utc = false;
            if (str.EndsWithIgnoreCase(" UTC"))
            {
                utc = true;
                str = str[0..^4];
            }

            if (!DateTime.TryParse(str, out var dt) &&
                !(str.Contains('-') && DateTime.TryParseExact(str, "yyyy-M-d", null, DateTimeStyles.None, out dt)) &&
                !(str.Contains('/') && DateTime.TryParseExact(str, "yyyy/M/d", null, DateTimeStyles.None, out dt)) &&
                !DateTime.TryParseExact(str, "yyyyMMddHHmmss", null, DateTimeStyles.None, out dt) &&
                !DateTime.TryParseExact(str, "yyyyMMdd", null, DateTimeStyles.None, out dt) &&
                !DateTime.TryParse(str, out dt))
            {
                dt = defaultValue;
            }

            // 处理UTC
            if (utc) dt = new DateTime(dt.Ticks, DateTimeKind.Utc);

            return dt;
        }

        // 特殊处理整数，Unix秒，绝对时间差，不考虑UTC时间和本地时间。
        if (value is Int32 k)
        {
            return k >= _maxSeconds || k <= -_maxSeconds ? defaultValue : _dt1970.AddSeconds(k);
        }

        if (value is Int64 m)
        {
            return m >= _maxMilliseconds || m <= -_maxMilliseconds
                ? defaultValue
                : m > 100 * 365 * 24 * 3600L
                    ? _dt1970.AddMilliseconds(m)
                    : _dt1970.AddSeconds(m);
        }

        try
        {
            return Convert.ToDateTime(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="defaultValue">默认值。待转换对象无效时使用</param>
    /// <returns></returns>
    public virtual DateTimeOffset ToDateTimeOffset(Object value, DateTimeOffset defaultValue)
    {
        if (value is DateTimeOffset num) return num;
        if (value == null || value == DBNull.Value) return defaultValue;

        // 特殊处理字符串，也是最常见的
        if (value is String str ||
            (value is IList<String> list && list.Count > 0 && (str = list[0]) != null))
        {
            str = str.Trim();
            if (str.IsNullOrEmpty()) return defaultValue;

            if (DateTimeOffset.TryParse(str, out var dt)) return dt;
            return str.Contains('-') && DateTimeOffset.TryParseExact(str, "yyyy-M-d", null, DateTimeStyles.None, out dt)
                ? dt
                : str.Contains('/') && DateTimeOffset.TryParseExact(str, "yyyy/M/d", null, DateTimeStyles.None, out dt)
                    ? dt
                    : DateTimeOffset.TryParseExact(str, "yyyyMMddHHmmss", null, DateTimeStyles.None, out dt)
                        ? dt
                        : DateTimeOffset.TryParseExact(str, "yyyyMMdd", null, DateTimeStyles.None, out dt)
                            ? dt
                            : defaultValue;
        }

        // 特殊处理整数，Unix秒，绝对时间差，不考虑UTC时间和本地时间。
        if (value is Int32 k)
        {
            return k >= _maxSeconds || k <= -_maxSeconds ? defaultValue : _dto1970.AddSeconds(k);
        }

        if (value is Int64 m)
        {
            return m >= _maxMilliseconds || m <= -_maxMilliseconds
                ? defaultValue
                : m > 100 * 365 * 24 * 3600L
                    ? _dto1970.AddMilliseconds(m)
                    : _dto1970.AddSeconds(m);
        }

        try
        {
            return Convert.ToDateTime(value);
        }
        catch
        {
            return defaultValue;
        }
    }


    /// <summary>去掉时间日期秒后面部分，可指定毫秒ms、分m、小时h</summary>
    /// <param name="value">时间日期</param>
    /// <param name="format">格式字符串，默认s格式化到秒，ms格式化到毫秒</param>
    /// <returns></returns>
    public virtual DateTime Trim(DateTime value, String format)
    {
        return format switch
        {
#if NET7_0_OR_GREATER
            "us" => new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Microsecond, value.Kind),
#endif
            "ms" => new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second,
                value.Millisecond, value.Kind),
            "s" => new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Kind),
            "m" => new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind),
            "h" => new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Kind),
            _ => value,
        };
    }

    /// <summary>时间日期转为yyyy-MM-dd HH:mm:ss完整字符串</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="useMillisecond">是否使用毫秒</param>
    /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
    /// <returns></returns>
    public virtual String ToFullString(DateTime value, Boolean useMillisecond, String emptyValue = null)
    {
        if (emptyValue != null && value <= DateTime.MinValue) return emptyValue;

        //return value.ToString("yyyy-MM-dd HH:mm:ss");

        //var dt = value;
        //var sb = new StringBuilder();
        //sb.Append(dt.Year.ToString().PadLeft(4, '0'));
        //sb.Append("-");
        //sb.Append(dt.Month.ToString().PadLeft(2, '0'));
        //sb.Append("-");
        //sb.Append(dt.Day.ToString().PadLeft(2, '0'));
        //sb.Append(" ");

        //sb.Append(dt.Hour.ToString().PadLeft(2, '0'));
        //sb.Append(":");
        //sb.Append(dt.Minute.ToString().PadLeft(2, '0'));
        //sb.Append(":");
        //sb.Append(dt.Second.ToString().PadLeft(2, '0'));

        //return sb.ToString();

        var cs = useMillisecond ? "yyyy-MM-dd HH:mm:ss.fff".ToCharArray() : "yyyy-MM-dd HH:mm:ss".ToCharArray();

        var k = 0;
        var y = value.Year;
        cs[k++] = (Char)('0' + (y / 1000));
        y %= 1000;
        cs[k++] = (Char)('0' + (y / 100));
        y %= 100;
        cs[k++] = (Char)('0' + (y / 10));
        y %= 10;
        cs[k++] = (Char)('0' + y);
        k++;

        var m = value.Month;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Day;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Hour;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Minute;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Second;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));

        if (useMillisecond)
        {
            k++;
            m = value.Millisecond;
            cs[k++] = (Char)('0' + (m / 100));
            cs[k++] = (Char)('0' + (m % 100 / 10));
            cs[k++] = (Char)('0' + (m % 10));
        }

        var str = new String(cs);

        // 此格式不受其它工具识别只存不包含时区的格式
        // 取出后，业务上存的是utc取出来再当utc即可
        //if (value.Kind == DateTimeKind.Utc) str += " UTC";

        return str;
    }

    /// <summary>时间日期转为yyyy-MM-dd HH:mm:ss完整字符串</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="useMillisecond">是否使用毫秒</param>
    /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
    /// <returns></returns>
    public virtual String ToFullString(DateTimeOffset value, Boolean useMillisecond, String emptyValue = null)
    {
        if (emptyValue != null && value <= DateTimeOffset.MinValue) return emptyValue;

        //var cs = "yyyy-MM-dd HH:mm:ss +08:00".ToCharArray();
        var cs = useMillisecond
            ? "yyyy-MM-dd HH:mm:ss.fff +08:00".ToCharArray()
            : "yyyy-MM-dd HH:mm:ss +08:00".ToCharArray();

        var k = 0;
        var y = value.Year;
        cs[k++] = (Char)('0' + (y / 1000));
        y %= 1000;
        cs[k++] = (Char)('0' + (y / 100));
        y %= 100;
        cs[k++] = (Char)('0' + (y / 10));
        y %= 10;
        cs[k++] = (Char)('0' + y);
        k++;

        var m = value.Month;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Day;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Hour;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Minute;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        m = value.Second;
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;

        if (useMillisecond)
        {
            m = value.Millisecond;
            cs[k++] = (Char)('0' + (m / 100));
            cs[k++] = (Char)('0' + (m % 100 / 10));
            cs[k++] = (Char)('0' + (m % 10));
            k++;
        }

        // 时区
        var offset = value.Offset;
        cs[k++] = offset.TotalSeconds >= 0 ? '+' : '-';
        m = Math.Abs(offset.Hours);
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));
        k++;
        m = Math.Abs(offset.Minutes);
        cs[k++] = (Char)('0' + (m / 10));
        cs[k++] = (Char)('0' + (m % 10));

        return new String(cs);
    }

    /// <summary>时间日期转为指定格式字符串</summary>
    /// <param name="value">待转换对象</param>
    /// <param name="format">格式化字符串</param>
    /// <param name="emptyValue">字符串空值时显示的字符串，null表示原样显示最小时间，String.Empty表示不显示</param>
    /// <returns></returns>
    public virtual String ToString(DateTime value, String format, String emptyValue)
    {
        if (emptyValue != null && value <= DateTime.MinValue) return emptyValue;

        //return value.ToString(format ?? "yyyy-MM-dd HH:mm:ss");

        return format.IsNullOrEmpty() || format == "yyyy-MM-dd HH:mm:ss"
            ? ToFullString(value, false, emptyValue)
            : value.ToString(format);
    }
}