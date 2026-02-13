using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
                AddTag(tag);

                // Write normalized tag back into output
                _builder.Append("{{").Append(tag).Append("}}");

                // Move past the tag
                i = end + 2;
            }
        }
        // Change your property from List to HashSet
        public HashSet<string> Tags { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> TagsNotReplaced { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private string AddTag(string tag)
        {
            // No 'if' or 'Contains' needed. 
            // If it's a duplicate, .Add() simply does nothing.
            Tags.Add(tag.Trim().ToLower());
            return tag;
        }

        private static string NormalizeTag(string tag)
        {
            return tag.Trim().ToLowerInvariant();
        }
        public string Apply(object model, string tagNotFoundTemplate = "")
        {
            var map = BuildPropertyMap(model.GetType());
            var output = Text;


            foreach (var tag in Tags)
            {
                var placeholder = tag.AddBrackets();
                var propertyName = tag.ToPropertyName();
                Console.WriteLine($"propertyName = {propertyName}");
                
                if (map.TryGetValue(propertyName, out var getter))
                {
                    var value = getter(model);
                    output = output.Replace(placeholder, value);
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