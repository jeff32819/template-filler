using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Jeff32819DLL.TemplateFiller20
{
    public sealed class TagParser
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public TagParser(string input)
        {
            Parse(input);
        }
        public bool AllTagsReplaced => TagsNotReplaced.Count == 0;
        public string Text => _builder.ToString();
        private void Parse(string input)
        {
            var i = 0;
            var length = input.Length;

            while (i < length)
            {
                var start = input.IndexOf("{{", i, StringComparison.Ordinal);
                if (start < 0)
                {
                    // No more tags
                    _builder.Append(input, i, length - i);
                    break;
                }

                // Append text before tag
                _builder.Append(input, i, start - i);

                var end = input.IndexOf("}}", start + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    throw new Exception("Closing brackets not found");
                }

                // Extract tag
                var tag = input.Substring(start + 2, end - (start + 2));
                tag = NormalizeTag(tag);
                // Add normalized tag to list
                Tags.Add(tag);

                // Write normalized tag back into output
                _builder.Append("{{").Append(tag).Append("}}");

                // Move past the tag
                i = end + 2;
            }
        }
        public HashSet<string> Tags { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> TagsNotReplaced { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Normalizes the specified tag by trimming whitespace and converting it to lowercase invariant.
        /// </summary>
        /// <param name="tag">The tag to normalize.</param>
        /// <returns>The normalized tag as a lowercase invariant string with no leading or trailing whitespace.</returns>
        private static string NormalizeTag(string tag)
        {
            return tag.Trim().ToLowerInvariant();
        }
        /// <summary>
        /// If tag is not found you have have template to show it in output, for example: "<strong>{0}</strong>", default is replace tag with empty string.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tagNotFoundTemplate"></param>
        /// <returns></returns>
        public string Apply(object model, string tagNotFoundTemplate = "")
        {
            var map = BuildPropertyMap(model.GetType());
            var output = Text;

            foreach (var tag in Tags)
            {
                var placeholder = tag.AddBrackets();
                var value = GetNestedPropertyValue(model, tag);

                if (value != null)
                {
                    output = output.Replace(placeholder, value.ToString());
                }
                else
                {
                    TagsNotReplaced.Add(tag);
                    if (string.IsNullOrEmpty(tagNotFoundTemplate))
                    {
                        output = output.Replace(placeholder, "");
                        continue;
                    }
                    var formatted = string.Format(tagNotFoundTemplate, placeholder);
                    output = output.Replace(placeholder, formatted);
                }
            }
            return output;
        }
        public static object GetNestedPropertyValue(object obj, string path)
        {
            var current = obj;
            var parts = path.Split('.');

            foreach (var part in parts)
            {
                if (current == null)
                    return null;

                var propName = part.ToPropertyName(); // your snake_case → PascalCase helper

                var prop = current.GetType().GetProperty(
                    propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (prop == null)
                    return null;

                current = prop.GetValue(current);
            }
            return current;
        }
        private static Dictionary<string, Func<object, string>> BuildPropertyMap(Type type)
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