using System.Collections.Generic;

namespace Jeff32819DLL.TemplateFiller20.Models
{
    public class TextParseResult
    {
        public string Text { get; set; }
        public HashSet<string> Tags { get; } = new HashSet<string>();
    }
}