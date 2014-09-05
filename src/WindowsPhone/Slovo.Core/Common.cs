using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slovo.Core
{
    public class Common
    {
        public const char NewLineDelimiter = '\n';
        public const string TabSpace = "  ";
        public static Encoding VocabularyEncoding = Encoding.UTF8;
        public static int MaxLinesPerTextBlock = 3;

        /// <summary>
        ///  Identifies when direction does not have offset
        /// </summary>
        internal const int NoOffset = 1;        

        public static bool IsRussianLetter(char c)
        {
            return (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == 'ё';
        }

        public static bool IsEnglishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }
    }
}
