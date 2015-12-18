namespace Slovo.Generator.Vocabulary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Slovo.Core;
    using System.Text.RegularExpressions;

    class OjegovFormatter : Formatter
    {
        public readonly string RegexAlternameBReplace;
        public readonly string RegexAlternameIReplace;
        

        internal OjegovFormatter(string fullFileName, ITypeFormatter formatter)
            : base(fullFileName, formatter)
        {
            RegexAlternameIReplace = Common.NewLineDelimiter.ToString() + Common.TabSpace + RegexAlternameBReplace;
            RegexAlternameBReplace = this.typeFormatter.AlternateColorBegin + "${text}." + this.typeFormatter.AlternateColorEnd;
        }

        protected override void Load()
        {
            using (StreamReader sr = new StreamReader(fullFileName, Encoding.GetEncoding("Windows-1251")))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string delimiter = "</b>";
                    int delimiterPos = s.IndexOf(delimiter);
                    int endWord = delimiterPos + delimiter.Length;
                    int startTranslation = delimiterPos + delimiter.Length + 1;
                    if (delimiterPos < 0 || endWord == s.Length)
                    {
                        var iDelimiter = "<i>";
                        delimiterPos = s.IndexOf(iDelimiter);
 
                        // in some cases </b> is in the end of the string
                        if (delimiterPos > 0)
                        {
                            endWord = delimiterPos;
                            startTranslation = delimiterPos;
                        }
                        else
                        {
                            var spaceDelimiter = " ";
                            delimiterPos = s.IndexOf(spaceDelimiter);
                            endWord = delimiterPos;
                            startTranslation = delimiterPos;
                        }

                        s = s.Substring(0, s.Length - delimiter.Length);
                    }

                    string word = s.Substring(0, endWord);
                    string translation = s.Substring(startTranslation);
                    System.Diagnostics.Trace.Assert(!string.IsNullOrWhiteSpace(word));
                    System.Diagnostics.Trace.Assert(!string.IsNullOrWhiteSpace(translation));
                    word = word.Replace("<b>", "");
                    word = word.Replace("</b>", "");
                    word = word.Replace(",", "");
                    word = word.Trim();
                    if (translation.StartsWith(","))
                    {
                        translation = translation.Substring(1);
                    }

                    // all wordds in ojegov dictionary are in upper case.
                    // Lower them all for now, as most of the words are in lower case
                    // ToDo: Determine which words are in Sentence case and update them
                    articles.Add(new Article(word.ToLower(), translation));
                }
            }
        }

        
        protected override string GetFormattedDefinition(string word, string translation)
        {
            //structure:
            //  1. white text
            //     <gray> .... </gray>
            //     <gray> .... </gray>
            //  2.
            // ||
            // ||

            
            translation = translation.Trim();
            translation = RemoveSingleXmlMarkups(translation, word);
            translation = translation.Replace(Deprecated7ThCharacter, "-");

            // new lines
            translation = Regex.Replace(translation, "(?<num>[0-9]+)\\.", Common.NewLineDelimiter + "${num}.");
            translation = translation.Replace("II", Common.NewLineDelimiter.ToString());
            

            // alternate color + new lines for <i>..</i>
            translation = Regex.Replace(translation, "<[i]>(?<text>.*?)</[i]>", RegexAlternameIReplace);
            translation = Regex.Replace(translation, "<[b]>(?<text>.*?)</[b]>", RegexAlternameBReplace);

            // repace М. -> Мечта
            string shordWord = (word[0] + ".");
            translation = translation.Replace(shordWord.ToUpper(), word);
            translation = translation.Replace(shordWord.ToLower(), word);

            // remove unpair
            translation = translation.Replace("<i>", "");
            translation = translation.Replace("</i>", "");
            translation = translation.Replace("<b>", "");
            translation = translation.Replace("</b>", "");

            // for some reasons some sentences are finished with .. instead of .
            translation = translation.Replace("..", ".");
            return translation;
        }

        private string RemoveSingleXmlMarkups(string str, string word)
        {
            string result = str;
            if (!string.IsNullOrEmpty(result))
            {
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    if (
                        // open <
                        (
                            result[i] == '<' 
                        
                            && 
                            (
                                (
                                    i < str.Length - 1 && result[i + 1] != 'b' && result[i + 1] != 'i' && result[i + 1] != '/'
                                )
                                || 
                                    i == str.Length - 1
                            )
                        )
                        ||
                            // closed >
                            (result[i] == '>'
                                &&
                                (
                                    (
                                        i != 0 && result[i - 1] != 'b' && result[i - 1] != 'i'
                                    )
                                    || 
                                        i == 0
                                 )
                            )
                        || 
                            (
                                result[i] == '&'
                            )
                        )
                        
                    {
                        // Console.WriteLine("Removing xml symbol {0} in {1}", result[i], word);
                        result = result.Remove(i, 1);
                    }
                }
            }

            return result;
        }


        protected override string GetShortDefinitionRegex()
        {
            return MullerFormatter.RussianShortDefinitionRegex;
        }

        protected override bool IsCurrentLanguage(char ch)
        {
            return Common.IsRussianLetter(ch);
        }
    }
}
