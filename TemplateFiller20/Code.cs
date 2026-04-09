using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Jeff32819DLL.TemplateFiller20.Models;

namespace Jeff32819DLL.TemplateFiller20
{
    public static class Code
    {
        /// <summary>
        ///     Normalizes the specified tag by trimming whitespace and converting it to lowercase invariant.
        /// </summary>
        /// <param name="tag">The tag to normalize.</param>
        /// <returns>The normalized tag as a lowercase invariant string with no leading or trailing whitespace.</returns>
        public static string NormalizeTag(string tag) => tag.Trim().ToLowerInvariant();

        /// <summary>
        /// Converts a string with words separated by underscores to PascalCase format.
        /// </summary>
        /// <remarks>PascalCase format capitalizes the first letter of each word and removes underscores.
        /// For example, "example_string" becomes "ExampleString".</remarks>
        /// <param name="input">The input string containing words separated by underscores. Cannot be null.</param>
        /// <returns>A string in PascalCase format, with each word capitalized and concatenated. Returns the original string if
        /// it is null or empty.</returns>
        public static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var words = input.Split('_');
            var sb = new StringBuilder();

            foreach (var word in words)
            {
                if (word.Length <= 0)
                {
                    continue;
                }
                // Capitalize the first letter and append the rest of the word
                sb.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                {
                    sb.Append(word.Substring(1).ToLower());
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// Converts the specified string to snake_case format by inserting underscores before uppercase letters and
        /// converting all characters to lowercase.
        /// </summary>
        /// <remarks>This method does not modify the original string. Underscores are inserted before each
        /// uppercase letter except the first character.</remarks>
        /// <param name="input">The input string to convert to snake_case. Cannot be null.</param>
        /// <returns>A new string in snake_case format. Returns the original string if it is null or empty.</returns>
        public static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (char.IsUpper(c))
                {
                    // If it's uppercase and NOT the first character, add an underscore
                    if (i > 0)
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLower(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// Retrieves the value of a nested property from an object using a dot-delimited property path.
        /// </summary>
        /// <remarks>If any property in the specified path does not exist or its value is null, the method
        /// returns null. Property names in the path are matched case-insensitively and may be converted from snake_case
        /// to PascalCase as needed.</remarks>
        /// <param name="obj">The object from which to retrieve the nested property value. Must not be null if a value is to be retrieved.</param>
        /// <param name="path">A dot-delimited string specifying the path to the nested property (e.g., "Address.Street.Name"). Each
        /// segment is matched to a property name, case-insensitively.</param>
        /// <returns>The value of the nested property if found; otherwise, null.</returns>
        public static object GetNestedPropertyValue(object obj, string path)
        {
            var current = obj;
            var parts = path.Split('.');

            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }

                var propName = part.ToPropertyName(); // your snake_case → PascalCase helper

                var prop = current.GetType().GetProperty(
                    propName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                );

                if (prop == null)
                {
                    return null;
                }

                current = prop.GetValue(current);
            }

            return current;
        }
        /// <summary>
        /// Builds a dictionary that maps property names of the specified type to functions that retrieve their string
        /// representations from an object instance.
        /// </summary>
        /// <remarks>Only public instance properties that can be read are included in the map. If a
        /// property's value is null, the function returns an empty string.</remarks>
        /// <param name="type">The type whose public instance properties will be included in the property map.</param>
        /// <returns>A dictionary where each key is a property name in lowercase, and each value is a function that takes an
        /// object instance and returns the string representation of the corresponding property value. The dictionary is
        /// case-insensitive with respect to property names.</returns>
        public static Dictionary<string, Func<object, string>> BuildPropertyMap(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(
                    p => p.Name,
                    p => (Func<object, string>)(obj =>
                    {
                        if (obj == null) return string.Empty;

                        // Standard 2.0: Use null for the index parameter
                        object value = p.GetValue(obj, null);

                        return value != null ? value.ToString() : string.Empty;
                    }),
                    StringComparer.OrdinalIgnoreCase
                );
        }

        



        public static TextParseResult ParseText(string input)
        {
            var result = new TextParseResult();

            if (string.IsNullOrEmpty(input))
            {
                result.Text = string.Empty;
                return result;
            }

            // Using a local StringBuilder for the text transformation
            var sb = new StringBuilder();
            var i = 0;
            var length = input.Length;

            while (i < length)
            {
                var start = input.IndexOf("{{", i, StringComparison.Ordinal);
                if (start < 0)
                {
                    sb.Append(input, i, length - i);
                    break;
                }

                // Append text before tag
                sb.Append(input, i, start - i);

                var end = input.IndexOf("}}", start + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    throw new Exception("Closing brackets not found at index " + start);
                }

                // 1. Extract and Clean
                var rawTag = input.Substring(start + 2, end - (start + 2));
                var normalizedTag = Code.NormalizeTag(rawTag.Trim()).Replace("_", "");

                if (!string.IsNullOrEmpty(normalizedTag))
                {
                    // 2. Add to the HashSet (it handles duplicates automatically)
                    result.Tags.Add(normalizedTag);

                    // 3. Write normalized tag back into output builder
                    sb.Append("{{").Append(normalizedTag).Append("}}");
                }
                else
                {
                    // Handle empty tags like {{  }}
                    sb.Append("{{}}");
                }

                i = end + 2;
            }

            result.Text = sb.ToString();
            return result;
        }
    }
}
