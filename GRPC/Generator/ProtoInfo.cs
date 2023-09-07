using System.Collections;
using System.Reflection;
using Reformat.Framework.Core.GRPC.Utils;
using TypeInfo = Reformat.Framework.Core.GRPC.Generator.TypeInfo;

namespace Reformat.Framework.Core.GRPC.Generator;

public class ProtoInfo
{
    internal Type ServiceType;
    internal Dictionary<string, Type> Enums = new Dictionary<string, Type>();

    internal Dictionary<string, TypeInfo> ClassTypes = new Dictionary<string, TypeInfo>();
    internal string Namespace;
    internal string PackageName;
    internal string ServiceName;

    /// <summary>
    /// 转换格式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    string convertType(Type type)
    {
        if (Nullable.GetUnderlyingType(type) != null)
        {
            //Nullable<T> 可空属性
            var type2 = type.GenericTypeArguments[0];
            return convertType(type2);
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            return $"map<{convertType(keyType)}, {convertType(valueType)}>";
        }
        else if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var keyType = type.GenericTypeArguments[0];
            return $"repeated {convertType(keyType)}";
        }

        if (type.IsClass && type != typeof(string))
        {
            return type.Name + "DTO";
        }

        if (type.IsEnum)
        {
            return type.Name + "DTO";
        }

        //https://blog.csdn.net/jadeshu/article/details/79183909
        switch (type.Name)
        {
            case nameof(Boolean):
                return "bool";
            case nameof(Decimal):
                return "double";
            case nameof(Single):
                return "float";
            case nameof(Guid):
                return "string";
            case nameof(DateTime):
                return "string";
        }

        return type.Name.ToLower();
    }

    /// <summary>
    /// 输出代码
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string CreateCode()
    {
        //var packageName = ServiceType.Name;
        var lines = new List<string>();
        lines.Add("syntax = \"proto3\";");
        lines.Add($"option csharp_namespace = \"{Namespace}\";");
        lines.Add($"package {PackageName};");

        //var serviceName = ServiceType.Name;
        //methods
        lines.Add("service " + ServiceName + " {");
        foreach (var method in ServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.DeclaredOnly))
        {
            var arguments = method.GetParameters();
            if (arguments.Length > 1)
            {
                throw new Exception($"{method.Name} 参数不能大于一个");
            }

            var args = new List<string>();
            foreach (var f in arguments)
            {
                var parameType = f.ParameterType;
                if (!parameType.IsClass || parameType == typeof(string) ||
                    typeof(IEnumerable).IsAssignableFrom(parameType))
                {
                    throw new Exception($"{ServiceType.Name}.{method.Name} 参数必须为class");
                }

                args.Add(convertType(parameType));
                ProtoUtils.CreateTypeInfo(this, parameType);
            }

            var returnType = method.ReturnType;
            if (!returnType.IsClass || returnType == typeof(string) ||
                typeof(IEnumerable).IsAssignableFrom(returnType))
            {
                throw new Exception($"{ServiceType.Name}.{method.Name} 返回类型必须为class");
            }

            ProtoUtils.CreateTypeInfo(this, returnType);

            //like  rpc SayHello (HelloRequest) returns (HelloReply);
            lines.Add($"    rpc {method.Name}({string.Join(",", args)}) returns ({convertType(returnType)});");
        }

        lines.Add("}"); //end service
        //enum
        foreach (var kv in Enums)
        {
            var type = kv.Value;
            lines.Add("message " + type.Name + "DTO {");
            foreach (var e in Enum.GetValues(type))
            {
                lines.Add($"    {e} = {(int)e};");
            }

            lines.Add("}");
        }

        //types
        foreach (var kv in ClassTypes)
        {
            var typeInfo = kv.Value;
            lines.Add("message " + typeInfo.Type.Name + "DTO {");
            int i = 0;
            foreach (var p in typeInfo.Fields)
            {
                i++;
                lines.Add($"    {convertType(p.Type)} {p.FieldName} = {i};");
            }

            lines.Add("}");
        }

        var code = string.Join("\r\n", lines);
        Console.WriteLine(code);
        var path = Environment.CurrentDirectory + "\\Protos";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        //path += $"\\{Namespace}";
        //if (!Directory.Exists(path))
        //    Directory.CreateDirectory(path);
        var file = path + $"\\{PackageName}.proto";
        File.WriteAllText(file, code);
        return code;
    }
}