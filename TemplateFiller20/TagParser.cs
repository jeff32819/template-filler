using System;
using System.Collections.Generic;
using System.Text;

using Jeff32819DLL.TemplateFiller20.Models;

namespace Jeff32819DLL.TemplateFiller20
{
    public sealed class TagParser
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public TagParser(string input)
        {
            Parse(input);
        }
        public HashSet<string> Tags { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
                tag = Code.NormalizeTag(tag);
                // Add normalized tag to list
                Tags.Add(tag);

                // Write normalized tag back into output
                _builder.Append("{{").Append(tag).Append("}}");

                // Move past the tag
                i = end + 2;
            }
        }



        /// <summary>
        ///     If tag is not found you have have template to show it in output, for example: "<strong>{0}</strong>", default is
        ///     replace tag with empty string.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tagNotFoundTemplate"></param>
        /// <returns></returns>
        public ApplyResult Apply(object model, string tagNotFoundTemplate = "")
        {
            var rv = new ApplyResult
            {
                Text = _builder.ToString()
            };
            var map = Code.BuildPropertyMap(model.GetType());
            foreach (var tag in Tags)
            {
                var placeholder = tag.AddBrackets();
                var value = Code.GetNestedPropertyValue(model, tag);

                if (value != null)
                {
                    rv.Text = rv.Text.Replace(placeholder, value.ToString());
                }
                else
                {
                    rv.TagsNotReplaced.Add(tag);
                    if (string.IsNullOrEmpty(tagNotFoundTemplate))
                    {
                        rv.Text = rv.Text.Replace(placeholder, "");
                        continue;
                    }

                    var formatted = string.Format(tagNotFoundTemplate, placeholder);
                    rv.Text = rv.Text.Replace(placeholder, formatted);
                }
            }
            return rv;
        }
    }
}