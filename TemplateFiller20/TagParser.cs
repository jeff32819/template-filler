using System;
using System.Text;
using Jeff32819DLL.TemplateFiller20.Models;

namespace Jeff32819DLL.TemplateFiller20
{
    public sealed class TagParser
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public TagDictionary TagDictionary { get; } = new TagDictionary();

        /// <summary>
        /// Parse text and extract all the tags
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="Exception"></exception>
        public void Parse(string input)
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
                TagDictionary.AddTag(tag);

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
        /// <param name="throwIfTagNotFound">Indicates whether to throw an exception if a tag is not found. Default is true.</param>
        /// <param name="tagNotFoundTemplate">Template to use when a tag is not found. Default is empty string.</param>
        /// <returns></returns>
        public ApplyResult Apply(bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            if (throwIfTagNotFound && TagDictionary.TagsWithoutValueCount() > 0)
            {
                throw new Exception("There are tags without values: " + string.Join(", ", TagDictionary.TagsWithoutValue()));
            }

            var rv = new ApplyResult
            {
                Text = _builder.ToString()
            };
            // not using at moment // var map = Code.BuildPropertyMap(model.GetType());


            foreach (var tag in TagDictionary.TagList())
            {
                var placeholder = tag.AddBrackets();
                TagDictionary.TryGetValue(tag, out var value);

                if (!string.IsNullOrEmpty(value))
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