using System;
using System.Collections.Generic;

namespace Jeff32819DLL.TemplateFiller20
{
    public class TextDictionary
    {
        public Dictionary<string, string> AllText = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        public void Add(string key, string text)
        {
            AllText.Add(key, text);
        }

        public void Add(string text)
        {
            if (AllText.Count > 0)
            {
                throw new Exception("There is already a default text parsed.");
            }

            AllText.Add("default", text);
        }
    }
}