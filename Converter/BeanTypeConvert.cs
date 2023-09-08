using System;
using System.ComponentModel;
using System.Linq;

namespace PublicUtility.Common.Lang.core;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class BeanTypeConvert
{
    /// <summary>
    /// 基于属性的对象转换 为 指定类型
    /// </summary>
    /// <param name="entity">源对象</param>
    /// <typeparam name="TConvert">目标类</typeparam>
    /// <returns></returns>
    public virtual TConvert ConvertTo<TConvert>(Object entity) where TConvert : new()
    {
        var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
        var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

        var convert = new TConvert();

        foreach (var entityProperty in entityProperties)
        {
            var property = entityProperty;
            var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
            if (convertProperty != null)
            {
                convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
            }
        }
        return convert;
    }
    [Obsolete("下扩展方法 setValueWithMath")]
    
    
    /// <summary>
    /// 基于属性匹配 的 深度拷贝 
    /// 类同 BeanUtils.copyProperties
    /// </summary>
    /// <param name="source">源对象</param>
    /// <param name="target">目标对象</param>
    /// <typeparam name="T">无需传参（自动判断）</typeparam>
    /// <typeparam name="TU"></typeparam>
    public virtual void CopyPropertiesTo<T, TU>(T source, ref TU target)
    {
        var sourceProps = typeof (T).GetProperties().Where(x => x.CanRead).ToList();
        var destProps = typeof(TU).GetProperties()
            .Where(x => x.CanWrite)
            .ToList();

        foreach (var sourceProp in sourceProps)
        {
            if (destProps.Any(x => x.Name == sourceProp.Name))
            {
                var p = destProps.First(x => x.Name == sourceProp.Name);
                if(p.CanWrite) { // check if the property can be set or no.
                    p.SetValue(target, sourceProp.GetValue(source, null), null);
                }
            }

        }
    }
}