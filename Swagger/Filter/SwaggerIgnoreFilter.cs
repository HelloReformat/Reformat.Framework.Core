using System.Reflection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Reformat.Framework.Core.Common.Extensions.lang;
using Reformat.Framework.Core.Swagger.Annotation;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Reformat.Framework.Core.Swagger.Filter;

public class SwaggerIgnoreFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext)
    {
        if (schema.Properties.Count > 0)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var memberList = schemaFilterContext.Type
                .GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(schemaFilterContext.Type.GetProperties(bindingFlags));

            var excludedList = memberList
                .Where(m => m.GetCustomAttribute<SwaggerIgnoreAttribute>() != null)
                .Select(m => (m.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? m.Name));

            foreach (var excludedName in excludedList)
            {
                string key = excludedName.ToCamelCase();
                if (schema.Properties.ContainsKey(key)) schema.Properties.Remove(key);
            }
        }
    }
}