using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Reformat.Framework.Core.Swagger;

/// <summary>
/// Swagger枚举类显示增强
/// </summary>
public class SwaggerEnumDisplayFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        schema.Enum.Clear();
        var type = context.Type;
        var str = new StringBuilder($"{schema.Description}(");
        foreach (var value in Enum.GetValues(type))
        {
            var fieldInfo = type.GetField(Enum.GetName(type, value));
            var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(true);
            schema.Enum.Add(OpenApiAnyFactory.CreateFromJson(JsonConvert.SerializeObject(value)));
            str.Append($"  {descriptionAttribute?.Description} : {(int)value}  、");
        }
        str.Append(')');
        schema.Description = str.ToString();
    }
}