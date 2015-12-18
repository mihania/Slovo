namespace Slovo.Generator
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Slovo.Core;

    internal class MullerFormatter : Formatter
    {
        internal const string RussianShortDefinitionRegex = @"[\p{IsCyrillic}]([\p{IsCyrillic}\s]){1,}[\p{IsCyrillic}]";

        internal MullerFormatter(string fullFileName, ITypeFormatter typeFormatter) : base(fullFileName, typeFormatter)
        {
        }

        protected override void Load()
        {
            int lineNumber = 0;
            using (StreamReader sr = new StreamReader(fullFileName, Encoding.GetEncoding("Windows-1251")))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    lineNumber++;

                    // before 293 there are helping words
                    if (lineNumber > 293)
                    {
                        string[] art = s.Split(new string[] { "  " }, 2, StringSplitOptions.None);
                        System.Diagnostics.Trace.Assert(art.Length == 2);
                        articles.Add(new Article(art[0], art[1]));
                    }
                }
            }
        }

        protected override string GetFormattedDefinition(string word, string originalDef)
        {
            //structure:
            //I.
            //  1. 
            //        1)
            //            a)
            //            б)
            //        2)
            //
            //  2.
            //II.

            string str1 = Regex.Replace(originalDef, "(?<num>[0-9]+)>", NewLineDelimiter + Common.TabSpace + "${num})");
            string str2 = Regex.Replace(str1, "(?<num>[а-я]+)>", NewLineDelimiter + Common.TabSpace + "${num})");
            string str3 = Regex.Replace(str2, "(?<num>[0-9]+)\\.", NewLineDelimiter + "${num}.");

            // roman numerals
            originalDef = Regex.Replace(str3, "_(?<num>[I, V, X]+)", NewLineDelimiter + "${num}.");
            originalDef = originalDef.Replace("_v.", "гл.");
            originalDef = originalDef.Replace("_n.", "сущ.");
            originalDef = originalDef.Replace("_a.", "прил.");
            originalDef = originalDef.Replace("_Syn:", NewLineDelimiter + "Syn:");
            originalDef = originalDef.Replace("_Ant:", NewLineDelimiter + "Ant:");
            originalDef = originalDef.Replace("_", "");
            originalDef = ReplaceSemicolon(originalDef);

            if (originalDef.StartsWith(NewLineDelimiter.ToString()))
            {
                originalDef = originalDef.Substring(NewLineDelimiter.ToString().Length);
            }

            originalDef = Decorate(originalDef);
            return originalDef;    
        }

        protected override string GetShortDefinitionRegex()
        {
            return RussianShortDefinitionRegex;
        }

        protected override bool IsCurrentLanguage(char ch)
        {
            return Common.IsEnglishLetter(ch);
        }
    }
}
