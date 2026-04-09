using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jeff32819DLL.TemplateFiller20
{
    /// <summary>
    ///     Represents a case-insensitive collection of tag-value pairs, providing methods to add, retrieve, and update tag
    ///     values.
    /// </summary>
    /// <remarks>
    ///     Tag names are compared using ordinal, case-insensitive comparison. Tags can be added with or
    ///     without associated values. Attempting to set the value of a tag that does not exist will not add a new
    ///     tag.
    /// </remarks>
    public class TagDictionary
    {
        /// <summary>
        ///     Internal dictionary to store tag-value pairs.
        /// </summary>
        public readonly Dictionary<string, string> AllTags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Adds a tag-value pair to the dictionary. If the tag already exists, its value is updated.
        /// </summary>
        /// <param name="tag">The tag to add or update.</param>
        public void AddTag(string tag)
        {
            var tmp = tag.Trim();
            Console.WriteLine(tmp);
            KeyList.Add(tmp);
            if (AllTags.ContainsKey(tag)) // do not want to overwrite existing values, check if the normalized tag already exists in the dictionary.
            {
                return;
            }
            AllTags.Add(tag, null);
            Console.WriteLine($"Adding tag: {tag}");
        }

        /// <summary>
        ///    Adds multiple tags to the dictionary. Each tag in the provided collection is added using the same logic as the AddTag method.
        /// </summary>
        /// <param name="tags">A collection of tags to add to the dictionary.</param>
        public void AddTags(HashSet<string> tags)
        {
            if (tags == null) return;

            foreach (var tag in tags)
            {
                // This re-uses your existing logic (normalization + check)
                this.AddTag(tag);
            }
        }
        public SortedSet<string> KeyList { get; } = new SortedSet<string>();

        /// <summary>
        ///     Try to get the value associated with the specified tag.
        /// </summary>
        /// <param name="tag">The tag whose value to get.</param>
        /// <param name="value">
        ///     When this method returns, contains the value associated with the specified tag, if the tag is
        ///     found; otherwise, null.
        /// </param>
        /// <returns>true if the tag was found; otherwise, false.</returns>
        public bool TryGetValue(string tag, out string value)
        {
            var normalizedTag = Code.NormalizeTag(tag);

            if (AllTags.TryGetValue(normalizedTag, out value))
            {
                // Key was found, 'value' is already set. 
                // Optional: handle nulls inside the dictionary itself
                if (value == null) value = string.Empty;
                return true;
            }

            // Key was NOT found

            value = string.Empty;
            return false;
        }

        /// <summary>
        ///     Set the value of the key, if the key does not exist, return false, otherwise set the value and return true.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetValue(string tag, string value)
        {
            var normalizedTag = Code.NormalizeTag(tag);
            if (char.IsUpper(tag[0])) // PascalCase tag, set value for both delimited versions of the tag (with '-' and '_') to support different naming conventions.
            {
                SetValueIfExists(Code.ToDelimitedCase(tag, '-'), value); // Set value for the tag with '-' delimiter if it exists in the dictionary.
                SetValueIfExists(Code.ToDelimitedCase(tag, '_'), value); // Set value for the tag with '_' delimiter if it exists in the dictionary.
            }
            SetValueIfExists(tag, value);
        }

        private void SetValueIfExists(string tag, string value)
        {
            tag = tag.ToLowerInvariant();
            if (!AllTags.ContainsKey(tag))
            {
                return;
            }
            AllTags[tag] = value;
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
            if (model == null)
            {
                return;
            }
            var props = Code.BuildPropertyMap(model.GetType());
            foreach (var entry in props)
            {
                SetValue(entry.Key.Replace("_", ""), entry.Value(model));
            }
        }


        /// <summary>
        ///     Gets tags without value.
        /// </summary>
        /// <returns></returns>
        public List<string> TagsWithoutValue()
        {
            return (from tag in AllTags where string.IsNullOrEmpty(tag.Value) select tag.Key).ToList();
        }
        /// <summary>
        ///     Determines whether there are any tags without a value in the collection.
        /// </summary>
        /// <returns></returns>
        public int TagsWithoutValueCount() => TagsWithoutValue().Count;



        /// <summary>
        ///     Determines whether any of the specified tags exist in the collection.
        /// </summary>
        /// <param name="tags">A sequence of tag strings to check for existence in the collection. Cannot be null.</param>
        /// <returns>true if at least one tag exists in the collection; otherwise, false.</returns>
        public bool AnyExists(IEnumerable<string> tags)
        {
            return tags.Any(tag => AllTags.ContainsKey(tag));
        }

        /// <summary>
        ///     Determines whether an entry with the specified tag exists in the collection.
        /// </summary>
        /// <param name="tag">The tag to locate in the collection. Cannot be null.</param>
        /// <returns>true if an entry with the specified tag exists; otherwise, false.</returns>
        public bool Exists(string tag)
        {
            return AllTags.ContainsKey(tag);
        }

        public void Debug()
        {
            foreach (var entry in AllTags)
            {
                var tag = entry.Key;
                var val = entry.Value;
                Console.WriteLine("Tag: " + tag + " = " + val);
            }
        }
    }
}