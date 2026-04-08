using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Adds a tag-value pair to the dictionary. If the tag already exists, its value is updated.
        /// </summary>
        /// <param name="tag">The tag to add or update.</param>
        /// <param name="value">The value associated with the tag. Can be null.</param>
        public void Add(string tag, string value = null)
        {
            var normalizedTag = Code.NormalizeTag(tag);
            _dict[normalizedTag] = value;
        }

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
            return _dict.TryGetValue(normalizedTag, out value);
        }

        /// <summary>
        ///     Set the value of the key, if the key does not exist, return false, otherwise set the value and return true.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string tag, string value)
        {
            var normalizedTag = Code.NormalizeTag(tag);
            if (!_dict.ContainsKey(normalizedTag))
            {
                return false;
            }

            _dict[normalizedTag] = value;
            return true;
        }

        /// <summary>
        ///     Gets tags without value.
        /// </summary>
        /// <returns></returns>
        public List<string> TagsWithoutValue()
        {
            return (from tag in _dict where string.IsNullOrEmpty(tag.Value) select tag.Key).ToList();
        }

        /// <summary>
        ///     Retrieves a list of all tag names currently stored in the collection.
        /// </summary>
        /// <returns>A list of strings containing the names of all tags. The list will be empty if no tags are present.</returns>
        public List<string> Tags()
        {
            return (from tag in _dict select tag.Key).ToList();
        }

        /// <summary>
        ///     Determines whether any of the specified tags exist in the collection.
        /// </summary>
        /// <param name="tags">A sequence of tag strings to check for existence in the collection. Cannot be null.</param>
        /// <returns>true if at least one tag exists in the collection; otherwise, false.</returns>
        public bool AnyExists(IEnumerable<string> tags)
        {
            return tags.Any(tag => _dict.ContainsKey(tag));
        }

        /// <summary>
        ///     Determines whether an entry with the specified tag exists in the collection.
        /// </summary>
        /// <param name="tag">The tag to locate in the collection. Cannot be null.</param>
        /// <returns>true if an entry with the specified tag exists; otherwise, false.</returns>
        public bool Exists(string tag)
        {
            return _dict.ContainsKey(tag);
        }
    }
}