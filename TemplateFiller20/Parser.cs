using System;
using System.Linq;

using static System.Net.Mime.MediaTypeNames;

namespace Jeff32819DLL.TemplateFiller20
{
    public class Parser
    {
        public TagDictionary Tags = new TagDictionary();

        public TextDictionary Texts = new TextDictionary();

        public Parser(bool throwIfTagNotFound = true, string tagNotFoundText = "")
        {
            ThrowIfTagNotFound = throwIfTagNotFound;
            TagNotFoundText = tagNotFoundText;
        }

        public bool ThrowIfTagNotFound { get; set; }
        public string TagNotFoundText { get; set; }

        public Parser AddText(string key, string text)
        {
            var result = Code.ParseText(text);
            Tags.AddTags(result.Tags);
            Texts.Add(key, text);
            return this;
        }

        public Parser AddText(string text)
        {
            if (Texts.AllText.Count > 0)
            {
                throw new Exception("There is already a default text parsed.");
            }
            var result = Code.ParseText(text);
            Tags.AddTags(result.Tags);
            Texts.Add("default", text);
            return this;
        }

        public Parser SetValues(object model)
        {
            Tags.SetValue(model);
            return this;
        }

        public Parser SetValue(string tag, string value)
        {
            Tags.SetValue(tag, value);
            return this;
        }

        public string Apply()
        {
            return Texts.AllText.Count > 1
                ? throw new Exception("there is more than one text, you must pass the name of one")
                : Process(Texts.AllText.First().Value);
        }

        public string Apply(string key)
        {
            return !Texts.AllText.TryGetValue(key, out var text)
                ? throw new Exception($"Cannot find key {key}")
                : Process(text);
        }

        private string Process(string text)
        {
            if (ThrowIfTagNotFound && Tags.TagsWithoutValueCount() > 0)
            {
                throw new Exception("There are tags without values: " + string.Join(", ", Tags.TagsWithoutValue()));
            }

            var rv = text ?? string.Empty;
            foreach (var tag in Tags.AllTags)
            {
                var placeholder = tag.Key.AddBrackets();
                var value = tag.Value;
                rv = rv.Replace(placeholder, !string.IsNullOrEmpty(value) ? value : TagNotFoundText);
            }

            return rv;
        }
    }
}