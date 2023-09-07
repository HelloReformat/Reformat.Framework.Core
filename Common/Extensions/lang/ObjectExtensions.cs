namespace Reformat.Framework.Core.Common.Extensions.lang;

public static class ObjectExtensions
{
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
}