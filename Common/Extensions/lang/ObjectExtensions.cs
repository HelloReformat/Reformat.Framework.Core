using PublicUtility.Common.Lang.core;

namespace Reformat.Framework.Core.Common.Extensions.lang;

public static class ObjectExtensions
{
    /// <summary>类型转换提供者</summary>
    public static BeanTypeConvert BeanTypeConvert { get; set; } = new BeanTypeConvert();
    
    /// <summary>
    /// 比较差异并拿出差异的地方
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second">对比两个对像哪个值不一样</param>
    /// <returns></returns>
    public static IDictionary<string, object> GetDiffentMap(this object first, object second)
    {
        IDictionary<string, object> _rz = new Dictionary<string, object>();
        Type _thistype = first.GetType();
        Type _secondtype = second.GetType();
        if (_thistype.FullName != _secondtype.FullName)
        {
            throw new Exception("逗逼两个类型不一样");
        }
        var _plist = _thistype.GetProperties();
        foreach (var _p in _plist)
        {
            dynamic _value1 = _p.GetValue(first);
            dynamic _value2 = _p.GetValue(second);
            if (_p.PropertyType.Name == typeof(string).Name)
            {
                if (_value1 == "")
                {
                    _value1 = null;
                }
                if (_value2 == "")
                {
                    _value2 = null;
                }
            }
            if (_value1 != _value2)
            {
                _rz.Add(_p.Name, _value2);
            }
        }
        return _rz;
    }
    
    /// <summary>
    /// 基于属性匹配的拷贝 适用于 Model赋值
    /// 类同 BeanUtils.copyProperties
    /// 深拷贝请使用 CopyEx
    /// </summary>
    /// <param name="source">源对象</param>
    /// <param name="target">目标对象</param>
    /// <typeparam name="T">无需传参（自动判断）</typeparam>
    /// <typeparam name="TU">无需传参（自动判断）</typeparam>
    public static void CopyPropertiesTo<T, TU>(this T source,ref TU target) => BeanTypeConvert.CopyPropertiesTo(source,ref target);
    public static TARGET CopyPropertiesTo<SOURCE,TARGET>(this SOURCE source) where TARGET : new()
    {
        TARGET obj = new TARGET();
        source.CopyPropertiesTo(ref obj);
        return obj;
    }
}