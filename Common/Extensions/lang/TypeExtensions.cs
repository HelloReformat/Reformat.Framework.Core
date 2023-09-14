using System.Reflection;

namespace Reformat.Framework.Core.Common.Extensions.lang;

public static class TypeExtensions
{
    public static TypeInfo GetTypeInfo(this Type type)
    {
        if (type == (Type) null) throw new ArgumentNullException(nameof (type));
        return type is IReflectableType reflectableType ? reflectableType.GetTypeInfo() : (TypeInfo) new TypeDelegator(type);
    }
    
    public static bool IsImplFrom(this Type entityType, Type interfaceType)
    {
        /*entityType.IsClass && !entityType.IsAbstract &&*/
        return  entityType.GetTypeInfo().GetInterfaces()
            .Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
    }

    public static bool IsSubClassOf(this Type entityType, Type superType)
    {
        return entityType.GetTypeInfo().IsSubclassOf(superType);
    }
}