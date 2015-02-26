using System.Diagnostics;
using Slovo.Core;

namespace Slovo.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [DebuggerDisplay("{Word}")]
    public class Article : IComparable<Article> 
    {
        public Article(string word, string translation)
        {
            this.Word = word;
            this.Translation = translation;
        }

        public string Word { get; set; }
        public string Translation { get; set; }
        public string ShortDefinition { get; set; }

        public int CompareTo(Article obj)
        {
            if (obj == null)
            {
                return 1;
            }

            return Common.StringCompare(this.Word, obj.Word); 
        }
    }
}
