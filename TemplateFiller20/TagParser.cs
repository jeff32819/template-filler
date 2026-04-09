using System;
using System.Collections.Generic;
using System.Linq;
using Jeff32819DLL.TemplateFiller20.Models;

namespace Jeff32819DLL.TemplateFiller20
{
    public sealed class TagParser
    {
        private readonly Dictionary<string, string> _textDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public TagDictionary TagDictionary { get; } = new TagDictionary();

        /// <summary>
        ///     Parse text and extract all the tags
        /// </summary>
        /// <param name="txt"></param>
        /// <exception cref="Exception"></exception>
        public void Parse(string txt)
        {
            if (_textDictionary.Count > 0)
            {
                throw new Exception("There is already a default text parsed.");
            }

            var result = Code.ParseText(txt);
            TagDictionary.AddTags(result.Tags);
            _textDictionary.Add("default", result.Text);
        }

        public void Parse(string key, string txt)
        {
            var result = Code.ParseText(txt);
            TagDictionary.AddTags(result.Tags);
            _textDictionary.Add(key, result.Text);
        }

        public ApplyResult Apply(string key, bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            return !_textDictionary.TryGetValue(key, out var text)
                ? throw new Exception($"Cannot find key {key}")
                : Process(text, throwIfTagNotFound, tagNotFoundTemplate);
        }

        public ApplyResult Apply(bool throwIfTagNotFound = true, string tagNotFoundTemplate = "")
        {
            return _textDictionary.Count > 1
                ? throw new Exception("there is more than one text, you must pass the name of one")
                : Process(_textDictionary.First().Value, throwIfTagNotFound, tagNotFoundTemplate);
        }

        /// <summary>
        ///     If tag is not found you have have template to show it in output, for example: "<strong>{0}</strong>", default is
        ///     replace tag with empty string.
        /// </summary>
        /// <param name="text"></param>
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
        public void SetValues(object model)
        {
            TagDictionary.SetValue(model);
        }

        public void Debug()
        {
            TagDictionary.Debug();
        }
    }
}