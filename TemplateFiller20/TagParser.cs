using System;
using System.Text;
using Jeff32819DLL.TemplateFiller20.Models;

namespace Jeff32819DLL.TemplateFiller20
{
    public sealed class TagParser
    {
        private readonly StringBuilder _builder = new StringBuilder();
        public string Text { get; private set; }
        public TagDictionary TagDictionary { get; } = new TagDictionary();

        /// <summary>
        ///     Parse text and extract all the tags
        /// </summary>
        /// <param name="txt"></param>
        /// <exception cref="Exception"></exception>
        public string Parse(string txt)
        {
            var result = Code.ParseText(txt);
            TagDictionary.AddTags(result.Tags);
            Text = (string.IsNullOrEmpty(Text) ? result.Text : Text + " " + result.Text).Trim();
            return result.Text;
        }

        public ApplyResult Apply(string text, bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            return Process(text, throwIfTagNotFound, tagNotFoundTemplate);
        }

        public ApplyResult Apply(bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            return Process(Text, throwIfTagNotFound, tagNotFoundTemplate);
        }

        /// <summary>
        ///     If tag is not found you have have template to show it in output, for example: "<strong>{0}</strong>", default is
        ///     replace tag with empty string.
        /// </summary>
        /// <param name="throwIfTagNotFound">Indicates whether to throw an exception if a tag is not found. Default is true.</param>
        /// <param name="tagNotFoundTemplate">Template to use when a tag is not found. Default is empty string.</param>
        /// <returns></returns>
        private ApplyResult Process(string text, bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            if (throwIfTagNotFound && TagDictionary.TagsWithoutValueCount() > 0)
            {
                throw new Exception("There are tags without values: " + string.Join(", ", TagDictionary.TagsWithoutValue()));
            }

            var rv = new ApplyResult
            {
                Text = text
            };
            // not using at moment // var map = Code.BuildPropertyMap(model.GetType());


            foreach (var tag in TagDictionary.TagList())
            {
                var placeholder = tag.AddBrackets();
                TagDictionary.TryGetValue(tag, out var value);

                if (!string.IsNullOrEmpty(value))
                {
                    rv.Text = rv.Text.Replace(placeholder, value);
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

        /// <summary>
        ///     Set the value of the key, if the key does not exist, return false, otherwise set the value and return true.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string tag, string value)
        {
            return TagDictionary.SetValue(tag, value);
        }

        /// <summary>
        ///     Set value for a dictionary item by using the property name and value of a model object.
        ///     The method uses reflection to get the properties of the model and their values, then sets the corresponding tags in
        ///     the dictionary with those values.
        ///     If a tag corresponding to a property does not exist in the dictionary, it will be ignored.
        /// </summary>
        /// <param name="model"></param>
        public void SetValue(object model)
        {
            TagDictionary.SetValue(model);
        }
    }
}