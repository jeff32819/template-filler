using System;
using System.Collections.Generic;

namespace Jeff32819DLL.TemplateFiller20.Models
{
    /// <summary>
    /// Result of apply operation, contains the output text and a list of tags that were not replaced (if any)
    /// </summary>
    public sealed class ApplyResult
    {
        /// <summary>
        /// text content associated with the object.
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// list of tags that were not replaced during the apply operation.
        /// </summary>
        public HashSet<string> TagsNotReplaced { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Indicates whether all tags were successfully replaced.
        /// </summary>
        public bool AllTagsReplaced => TagsNotReplaced.Count == 0;
    }
}