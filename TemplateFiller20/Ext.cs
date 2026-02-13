using System;
using System.Linq;

namespace Jeff32819DLL.TemplateFiller20
{
    public static class Ext
    {
        public static string AddBrackets(this string tag) => $"{{{{{tag}}}}}";
        public static string ToPropertyName(this string tag)
        {
            var parts = tag.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
        }
    }
}
