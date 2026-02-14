using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jeff32819DLL.TemplateFiller20
{
    internal class Code
    {
        /// <summary>
        ///     Normalizes the specified tag by trimming whitespace and converting it to lowercase invariant.
        /// </summary>
        /// <param name="tag">The tag to normalize.</param>
        /// <returns>The normalized tag as a lowercase invariant string with no leading or trailing whitespace.</returns>
        public static string NormalizeTag(string tag) => tag.Trim().ToLowerInvariant();

        public static object GetNestedPropertyValue(object obj, string path)
        {
            var current = obj;
            var parts = path.Split('.');

            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }

                var propName = part.ToPropertyName(); // your snake_case → PascalCase helper

                var prop = current.GetType().GetProperty(
                    propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (prop == null)
                {
                    return null;
                }

                current = prop.GetValue(current);
            }

            return current;
        }
        public static Dictionary<string, Func<object, string>> BuildPropertyMap(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(
                    p => p.Name.ToLowerInvariant(),
                    p => (Func<object, string>)(obj =>
                    {
                        var value = p.GetValue(obj);
                        return value?.ToString() ?? "";
                    }),
                    StringComparer.OrdinalIgnoreCase
                );
        }
    }
}
