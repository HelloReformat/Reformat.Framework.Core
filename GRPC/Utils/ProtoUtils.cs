using System.Reflection;
using Reformat.Framework.Core.GRPC.Attributes;
using Reformat.Framework.Core.GRPC.Generator;
using FieldInfo = Reformat.Framework.Core.GRPC.Generator.FieldInfo;
using TypeInfo = Reformat.Framework.Core.GRPC.Generator.TypeInfo;


namespace Reformat.Framework.Core.GRPC.Utils;

public static class ProtoUtils
{
    //static Dictionary<string, List<string>> nameSpaceCache = new Dictionary<string, List<string>>();
    public static List<ProtoInfo> Convert(params Assembly[] assemblies)
    {
        var infos = new List<ProtoInfo>();
        foreach (var asb in assemblies)
        {
            var allTypes = asb.GetTypes();
            foreach (var type in allTypes)
            {
                if (!type.IsAbstract)
                {
                    continue;
                }

                var p = type.GetCustomAttribute(typeof(ProtoSourceAttribute));
                if (p == null)
                {
                    continue;
                }

                var p2 = p as ProtoSourceAttribute;
                var info = new ProtoInfo();
                info.ServiceType = type;
                info.Namespace = p2.NameSpace;
                if (string.IsNullOrEmpty(info.Namespace))
                {
                    info.Namespace = $"gRPC.{type.Namespace}.{p2.PackageName}";
                }

                info.PackageName = p2.PackageName;
                info.ServiceName = p2.ServiceName;
                if (string.IsNullOrEmpty(info.ServiceName))
                {
                    info.ServiceName = "gRPC" + type.Name;
                }

                infos.Add(info);
            }
        }
        return infos;
    }


    internal static void CreateTypeInfo(ProtoInfo ProtoInfo, Type type)
    {
        if (type == typeof(object))
        {
            throw new Exception($"不能为object");
        }

        if (!type.IsClass || type == typeof(string))
        {
            return; //非class跳过
        }

        //if (Nullable.GetUnderlyingType(type) != null)
        //{
        //    //Nullable<T> 可空属性
        //    var type2 = type.GenericTypeArguments[0];
        //    CreateTypeInfo(ProtoInfo, type2);
        //    return;
        //}
        if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            CreateTypeInfo(ProtoInfo, keyType);
            CreateTypeInfo(ProtoInfo, valueType);
            return;
        }
        else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var keyType = type.GenericTypeArguments[0];
            CreateTypeInfo(ProtoInfo, keyType);
            return;
        }

        var pros = type.GetProperties();
        var typeInfo = new Generator.TypeInfo() { Type = type };

        //var a = nameSpaceCache.TryGetValue(ProtoInfo.Namespace, out var cacheTypes);
        //if (!a)
        //{
        //    cacheTypes = new List<string>();
        //    nameSpaceCache.Add(ProtoInfo.Namespace, cacheTypes);
        //}

        foreach (var p in pros)
        {
            typeInfo.Fields.Add(new Generator.FieldInfo() { FieldName = p.Name, Type = p.PropertyType });
            if (p.PropertyType.IsEnum)
            {
                if (!ProtoInfo.Enums.ContainsKey(p.PropertyType.FullName))
                {
                    //if (cacheTypes.Contains(p.PropertyType.FullName))
                    //{
                    //    return;
                    //}
                    //cacheTypes.Add(p.PropertyType.FullName);
                    ProtoInfo.Enums.Add(p.PropertyType.FullName, p.PropertyType);
                }
            }
            else if (p.PropertyType.IsClass && p.PropertyType != typeof(string))
            {
                CreateTypeInfo(ProtoInfo, p.PropertyType);
            }
        }

        if (!ProtoInfo.ClassTypes.ContainsKey(type.FullName))
        {
            //if (cacheTypes.Contains(type.FullName))
            //{
            //    return;
            //}
            //cacheTypes.Add(type.FullName);
            ProtoInfo.ClassTypes.Add(type.FullName, typeInfo);
        }
    }
}