using System;
using System.Linq;

namespace Jeff32819DLL.TemplateFiller20
{
    public class Parser
    {
        public TagDictionary Tags = new TagDictionary();

        public TextDictionary Texts = new TextDictionary();

        public Parser(string tagNotFoundText = "")
        {
            TagNotFoundText = tagNotFoundText;
        }
        public string TagNotFoundText { get; set; }
        public Parser AddText(string key, string text)
        {
            var result = Code.ParseText(text);
            Tags.AddTags(result.Tags);
            Texts.Add(key, result.Text);
            return this;
        }

        public Parser AddText(string text)
        {
            if (Texts.AllText.Count > 0)
            {
                throw new Exception("There is already a default text parsed.");
            }
            AddText("default", text);
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
        /// <summary>
        /// Validates that all tags have values, if any tag does not have a value, an exception is thrown with the list of tags without values. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Parser VerifyAllTagsHaveValue()
        {
            return Tags.TagsWithoutValueCount() > 0 ? throw new Exception("There are tags without values: " + string.Join(", ", Tags.TagsWithoutValue())) : this;
        }


        public string ParseTemplate()
        {
            return Texts.AllText.Count > 1
                ? throw new Exception("there is more than one text, you must pass the name of one")
                : Process(Texts.AllText.First().Value);
        }

        public string ParseTemplate(string key)
        {
            return !Texts.AllText.TryGetValue(key, out var text)
                ? throw new Exception($"Cannot find key {key}")
                : Process(text);
        }

        private string Process(string text)
        {
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