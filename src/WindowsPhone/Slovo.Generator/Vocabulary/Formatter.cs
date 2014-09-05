using System.Globalization;
using System.Xml;

namespace Slovo.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Text.RegularExpressions;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using Slovo.Generator.Direction;

    abstract class Formatter
    {
        protected List<Article> articles = new List<Article>();
        protected string fullFileName;

        private string listFileName;
        private string defFileName;

        protected const string AlternateColorBegin = "<Span Foreground=\"Gray\">";
        protected const string AlternateColorEnd = "</Span>";
        protected static string Deprecated7ThCharacter = char.ConvertFromUtf32(0x07);

        /// <summary>
        /// Full file name with short definitions
        /// </summary>
        private string shortDefFileName;

        private Regex regex;

        internal Formatter(string fullFileName)
        {
            this.fullFileName = fullFileName;
            string pattern = GetShortDefinitionRegex();

            this.regex = new Regex(
                pattern,
                RegexOptions.IgnoreCase
                | RegexOptions.Multiline
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                );

            string name = Path.GetFileNameWithoutExtension(fullFileName);
            this.listFileName = Path.Combine(OutputPath, name + Vocabulary<FileStreamGetter>.IndexFileExtension);
            this.defFileName = Path.Combine(OutputPath, name + Vocabulary<FileStreamGetter>.DataFileExtension);
            this.Load();
            articles.Sort();
            this.JoinDuplicates();
        }

        protected string OutputPath
        {
            get
            {
                return @"..\..\..\Slovo.UI\Data";
            }
        }


        internal List<Article> Articles
        {
            get
            {
                return this.articles;
            }
        }

        internal void WriteOutput()
        {
            using (FileStream listFileStream = new FileStream(listFileName, FileMode.Create)) 
            {
                using (BinaryWriter listBinaryWriter = new BinaryWriter(listFileStream))
                {
                    using (FileStream defFileStream = new FileStream(defFileName, FileMode.Create))
                    {
                        using (BinaryWriter defBinaryWriter = new BinaryWriter(defFileStream))
                        {
                            listBinaryWriter.Write(articles.Count);
                            foreach (Article article in articles)
                            {
                                var formattedDefinition = this.GetFormattedDefinition(article.Word, article.Translation);
                                if (IsValidXml(formattedDefinition, article))
                                {
                                    listBinaryWriter.Write(article.Word);
                                    if (defFileStream.Position > Int32.MaxValue)
                                    {
                                        throw new ApplicationException("Position is out of range");
                                    }

                                    listBinaryWriter.Write((int) defFileStream.Position);
                                    if (string.IsNullOrEmpty(article.ShortDefinition))
                                    {
                                        article.ShortDefinition = this.GetShortDefinition(article.Word,
                                            article.Translation);
                                    }

                                    defBinaryWriter.Write(article.ShortDefinition);
                                    defBinaryWriter.Write(formattedDefinition);
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Writing to file {0}", Path.GetFullPath(listFileName));
            Console.WriteLine("Writing to file {0}", Path.GetFullPath(defFileName));
        }

        protected string GetShortDefinition(string word, string translation) 
        {
            string result = string.Empty;
            Match m = regex.Match(translation);
            int currentGroup = 0;
            const int MaxLength = 20;
            StringBuilder sb = new StringBuilder();

            while (m.Success)
            {
                Group g = m.Groups[0];
                string valueToAdd = g.Value;

                if (sb.Length == 0 || (sb.Length + valueToAdd.Length < MaxLength))
                {
                    if (sb.Length != 0)
                    {
                        sb.Append("; ");
                    }

                    sb.Append(valueToAdd);

                    currentGroup++;
                }

                m = m.NextMatch();
            }

            result = sb.ToString().Trim();
            return result;

        }

        protected virtual string GetFormattedDefinition(string word, string originalDef)
        {
            return originalDef;
        }

        /// <summary>
        /// Gets formatted short definition for the given word
        /// </summary>
        /// <returns></returns>
        protected abstract string GetShortDefinitionRegex();

        protected abstract bool IsCurrentLanguage(char ch);

        protected abstract void Load();

        protected static string ReplaceSemicolon(string originalDef)
        {
            originalDef = originalDef.Replace("; " + Common.NewLineDelimiter, " " + Common.NewLineDelimiter);
            originalDef = originalDef.Replace(";" + Common.NewLineDelimiter, Common.NewLineDelimiter.ToString());
            originalDef = originalDef.Replace(";", Common.NewLineDelimiter + Common.TabSpace + Common.TabSpace);
            return originalDef;
        }

        protected string Decorate(string text)
        {
            StringBuilder result = new StringBuilder();

            using (StringReader reader = new StringReader(text))
            {
                string line;
                int lineCount = 0;
                StringBuilder buffer = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    buffer.Append(line);
                    buffer.Append(Common.NewLineDelimiter);
                    lineCount++;

                    if (lineCount == Common.MaxLinesPerTextBlock)
                    {
                        lineCount = 0;

                        this.DecorateBuffer(buffer, result);
                        buffer = new StringBuilder();
                    }
                }

                if (buffer.Length != 0)
                {
                    this.DecorateBuffer(buffer, result);
                    buffer = new StringBuilder();
                }
            }

            return result.ToString();
        }

        private void DecorateBuffer(StringBuilder buffer, StringBuilder result)
        {
            bool isEngOpened = false;
            int curly = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                char cur = buffer[i];
                char prev = (i > 0) ? buffer[i - 1] : '|';
                bool isCurEng = this.IsCurrentLanguage(cur);
                bool isPrevEng = this.IsCurrentLanguage(prev);
                
                if (isEngOpened)
                {
                    if (cur == '(')
                    {
                        curly++;
                    }
                    else if (cur == ')')
                    {
                        curly--;
                    }
                }

                if ((isCurEng || (buffer[i] == '(' && i < buffer.Length - 1 && this.IsCurrentLanguage(buffer[i + 1])))
                    && !isPrevEng && !isEngOpened)
                {
                    result.Append(AlternateColorBegin);
                    isEngOpened = true;
                    curly = 0;
                    if (cur == '(')
                    {
                        curly++;
                    }
                }
                else if (!isCurEng && !IsAllowedInBetween(cur, prev, curly)
                    && (isPrevEng || IsAllowedInBetween(prev, prev, curly))
                    && isEngOpened)
                {
                    result.Append(AlternateColorEnd);
                    isEngOpened = false;
                }

                result.Append(cur);
            }

            if (isEngOpened)
            {
                result.Append(AlternateColorEnd);
                isEngOpened = false;
            }
        }

        private static bool IsAllowedInBetween(char ch, char prevCh, int curly)
        {
            return
                (curly >= 0 || ch != ')') && (prevCh != ' ' || ch != '-') && ch != Common.NewLineDelimiter
                && (Char.IsPunctuation(ch) || Char.IsWhiteSpace(ch) || ch == '~' || Char.IsDigit(ch));
        }

        private void JoinDuplicates()
        {
            for (int i = Articles.Count - 1; i > 0; i--)
            {
                int startDuplicate = -1;
                for (int k = i - 1; k >= 0; k--) 
                {
                    if (Articles[i].Word == Articles[k].Word)
                    {
                        startDuplicate = k;
                    }
                    else
                    {
                        break;
                    }
                }

                string[] numbers = {"I", "II", "III", "IV"};

                if (startDuplicate != -1)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int k = startDuplicate; k <= i; k++)
                    {
                        if (k!= startDuplicate) 
                        {
                            sb.Append(Common.NewLineDelimiter);
                        }
                        sb.Append(numbers[k - startDuplicate]).Append(".").Append(" ").Append(Articles[k].Translation);
                    }

                    Articles[startDuplicate].Translation = sb.ToString();

                    for (int k = i; k > startDuplicate; k--)
                    {
                        Articles.RemoveAt(k);
                    }
                }
            }
        }

        private bool IsValidXml(string innerXml, Article article)
        {
            bool result = false;
            try
            {
                string xml = "<doc>" + innerXml + "</doc>";
                XmlDocument doc = new XmlDocument();

                // this call has to fail if xml is wrong
                doc.LoadXml(xml);
               
                result = true;
            }
            catch (XmlException ex)
            {
                throw new ApplicationException(article.Word + ": inccorect translation xml: " + ex.Message, ex);
            }

            return result;
        }
    }
}
