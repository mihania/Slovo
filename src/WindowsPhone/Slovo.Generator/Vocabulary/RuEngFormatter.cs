namespace Slovo.Generator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System;
    using Slovo.Core;

    internal class RuEngFormatter : Formatter
    {
        internal const string EnglishSentencesRegexPattern = @"[a-zA-Z]([a-zA-Z\s]){1,}[a-zA-Z]";
        internal RuEngFormatter(string fullFileName) : base(fullFileName)
        {
        }

        protected override void Load()
        {
            Encoding encoding = Encoding.UTF8;

            using (StreamReader sr = new StreamReader(fullFileName, Encoding.Default))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string[] article = s.Split(new string[] { "  " }, 2, StringSplitOptions.None);
                    System.Diagnostics.Trace.Assert(article.Length == 2);

                    if (article.Length < 2)
                    {
                        int ind = s.IndexOf(' ');
                        if (ind <= 0)
                        {
                            continue;
                        }
                        article = new string[2];
                        article[0] = s.Substring(0, ind);
                        article[1] = s.Substring(ind + 1);

                    }

                    int posT = article[0].IndexOf('|');
                    if (posT > 0)
                    {
                        string newArticle0 = article[0].Substring(0, posT);
                        if (posT < article[0].Length)
                            article[0] = newArticle0 + article[0].Substring(posT + 1);
                    }

                    articles.Add(new Article(article[0], article[1]));
                }
            }

            this.JoinRomanDuplicates();
        }

        protected override string GetFormattedDefinition(string word, string originalDef)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(originalDef);
            string tab = Common.NewLineDelimiter + "\t";
            string syn = "_Syn:";


            for (int i = 0; i < sb.Length; i++)
            {
                bool isBigger = sb[i] == '>';
                bool isDot = false;
                if (isBigger)
                {
                }
                else
                {
                    isDot = sb[i] == '.';
                }

                if (isBigger || isDot)
                {
                    int j = i - 1;
                    while (j > 0 && Char.IsNumber(sb[j]))
                    {
                        j--;
                    }
                    if (j > 0 && j != i - 1)
                    {
                        if (isBigger)
                        {
                            sb.Insert(j, tab);
                            i += tab.Length;
                        }
                        else if (isDot)
                        {
                            sb.Insert(j, Common.NewLineDelimiter);
                            i += Common.NewLineDelimiter.ToString().Length;
                        }
                    }
                }
                else
                {
                    // "_Syn:"
                    if (i > syn.Length
                        && sb[i] == ':'
                        && sb[i - 1] == 'n'
                        && sb[i - 2] == 'y'
                        && sb[i - 3] == 'S'
                        && sb[i - 4] == '_')
                    {
                        sb.Insert(i - syn.Length, Common.NewLineDelimiter);
                        i += Common.NewLineDelimiter.ToString().Length;
                    }
                }
            }

            string result = ReplaceSemicolon(sb.ToString());
            if (result.StartsWith(","))
            {
                result = result.Substring(1);
            }
            
            // substitute ~ with full word if it separate
            result = result.Replace(" ~ ", " " + word + " ");
            
            // add - between russian phrase and english translatio
            result = Regex.Replace(result, "(?<ruLetter>[а-яА-Я]+)\\ (?<enLetter>[a-zA-Z]+)", "${ruLetter} - ${enLetter}");

            // remove * - it seems it used to show that this word is specified in the dictionary
            result = result.Replace("*", "");

            result = this.Decorate(result);
            return result;
        }

        protected override string GetShortDefinitionRegex()
        {
            return EnglishSentencesRegexPattern;
        }

        protected override bool IsCurrentLanguage(char ch)
        {
            return Common.IsRussianLetter(ch);
        }


        /// <summary>
        /// Joining words that differs only in roman number I, II, III, IV
        /// </summary>
        private void JoinRomanDuplicates()
        {
            string[] endings = { "I", "II", "III", "IV", "V" };
            for (int i = articles.Count - 1; i >= 0; i--)
            {
                Article line = articles[i];
                int endJoin = -1;
                int endingNumber = 0;
                int endingsEndNumber = -1;

                // looking if word matching template [{sense} {I|II|III|IV}]
                for (endingNumber = 0; endingNumber < endings.Length; endingNumber++)
                {
                    if (line.Word.EndsWith(" " + endings[endingNumber]))
                    {
                        endJoin = i;
                        endingsEndNumber = endingNumber;
                        break;
                    }
                }

                if (endJoin != -1)
                {

                    while (--i >= 0)
                    {
                        endingNumber--;
                        if (endingNumber < 0 || !articles[i].Word.EndsWith(" " + endings[endingNumber]))
                        {
                            break;
                        }
                    }

                    int startJoin = i + 1;

                    articles[startJoin].Word = articles[startJoin].Word.Substring(0, articles[startJoin].Word.Length - 2).TrimEnd();
                    articles[startJoin].Translation = endings[0] + "." + Common.NewLineDelimiter + "  " + articles[startJoin].Translation;

                    for (int k = startJoin + 1; k <= endJoin; k++)
                    {
                        articles[startJoin].Translation += Common.NewLineDelimiter + endings[k - startJoin] + "." + Common.NewLineDelimiter + "  " + articles[k].Translation;
                    }


                    for (int k = endJoin; k > startJoin; k--)
                    {
                        articles.RemoveAt(k);
                    }
                }
            }
        }
    }
}
