using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Slovo.Core;

namespace Slovo.Generator.Formatters
{

    internal class WordNet3_0Formatter : Formatter, IDisposable
    {
        internal const string indexAllFileName = @"..\..\InputData\wn\index.all";
        private const string dataAdjFileName = @"..\..\InputData\wn\data.adj";
        private const string dataAdvFileName = @"..\..\InputData\wn\data.adv";
        private const string dataNounFileName = @"..\..\InputData\wn\data.noun";
        private const string dataVerbFileName = @"..\..\InputData\wn\data.verb";

        private Dictionary<string, SeekReadStream> fileStreamDict = new Dictionary<string, SeekReadStream>() {
            {"a", new SeekReadStream(dataAdjFileName)},
            {"r", new SeekReadStream(dataAdvFileName)},
            {"n", new SeekReadStream(dataNounFileName)},
            {"v", new SeekReadStream(dataVerbFileName)}
        };


        internal WordNet3_0Formatter() : base("wordnet3_0")
        {
        }

        protected override void Load()
        {
            Dictionary<string, Def> dict = new Dictionary<string, Def>();
            using (StreamReader stream = new StreamReader(indexAllFileName))
            {
                string str;
                while ((str = stream.ReadLine()) != null)
                {
                    string[] wordDef = str.Split(new char[] { ' ' }, 2, StringSplitOptions.None);
                    string word = wordDef[0];
                    word = word.Replace('_', ' ');
                    string def = wordDef[1];

                    if (!dict.Keys.Contains(word))
                    {
                        dict[word] = new Def();
                    }

                    dict[word].LemmaList.Add(def);
                }

                foreach (KeyValuePair<string, Def> pair in dict)
                {
                    StringBuilder sb = new StringBuilder();
                    string shortDefinition = null;
                    int lemmaCount = 0;
                    foreach (string lemma in pair.Value.LemmaList)
                    {
                        lemmaCount++;
                        string[] spl = lemma.Split(new char[] { ' ' });
                        string pos = spl[0]; // 0
                        int synset_cnt = int.Parse(spl[1]); // 1
                        int p_cnt = int.Parse(spl[2]); // 2
                        int x = 2 + p_cnt + 1 + 1; // 2 - current, p_cnt skip, 1 - sense_cnt, 1 - tagsense_cnt
                        this.AppendDecoratedPos(sb, pos);
                        sb.Append(Common.NewLineDelimiter);
                        for (int i = 1; i <= synset_cnt; i++)
                        {
                            sb.Append(i).Append(".").Append(Common.NewLineDelimiter);
                            fileStreamDict[pos].Sr.BaseStream.Seek(long.Parse(spl[x + i]), SeekOrigin.Begin);
                            fileStreamDict[pos].Sr.DiscardBufferedData();
                            string[] lineArray = fileStreamDict[pos].Sr.ReadLine().Split(new char[] {'|'}, 2);
                            string plainDefinition = EncodeXml(lineArray[1]);
                            string def = DecorateString(plainDefinition);
                            shortDefinition = GetShortDefinition(pair.Key, plainDefinition);
                            sb.Append(def).Append(Common.NewLineDelimiter);
                        }
                    }

                    pair.Value.Value = sb.ToString();
                    var article = new Article(pair.Key, pair.Value.Value);
                    article.ShortDefinition = shortDefinition;
                    articles.Add(article);
                }
            }
        }

        private string EncodeXml(string xml)
        {
            string result;
            if (string.IsNullOrEmpty(xml))
            {
                result = xml;
            }
            else
            {
                result = xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");    
            }
            
            return result;
        }

        public void Dispose()
        {
            foreach (SeekReadStream fs in fileStreamDict.Values)
            {
                fs.Dispose();
            }   
        }

        protected override bool IsCurrentLanguage(char ch)
        {
            return Common.IsEnglishLetter(ch);
        }

        private string DecorateString(string input)
        {
            return Regex.Replace(input, @"(\"")(.*)(\"")", "<Span Foreground=\"Gray\">$2</Span>");
        }

        private void AppendDecoratedPos(StringBuilder result, string pos)
        {
            // ToDo: Define accent color
            const string engBegin = "<Span Foreground=\"Red\">";
            const string engEnd = "</Span>";
            result.Append(engBegin);
            if (pos == "a")
            {
                result.Append("Adjective");
            }
            else if (pos == "v")
            {
                result.Append("Verb");
            }
            else if (pos == "r")
            {
                result.Append("Adverb");
            }
            else if (pos == "n")
            {
                result.Append("Noun");
            }
            else
            {
                throw new ArgumentOutOfRangeException(pos);
            }

            result.Append(engEnd).ToString();
        }

        protected override string GetShortDefinitionRegex()
        {
            return RuEngFormatter.EnglishSentencesRegexPattern;
        }

        internal class Def
        {
            internal List<string> LemmaList = new List<string>();
            internal string Value;
        }

        internal class SeekReadStream : IDisposable
        {
            internal FileStream Fs;
            internal StreamReader Sr;
            internal SeekReadStream(string fileName)
            {
                Fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                Sr = new StreamReader(Fs);
            }

            public void Dispose()
            {
                Fs.Dispose();
                Sr.Dispose();
            }
        }
    }
}
