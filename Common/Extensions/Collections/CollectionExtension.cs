namespace Reformat.Framework.Core.Common.Extensions.Collections;

public static class CollectionExtension
{
    public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;

}