namespace Slovo.Generator.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Slovo.Core;

    internal class WordNet30Formatter : Formatter, IDisposable
    {
        internal const string indexAllFileName = @"..\..\InputData\wn\index.all";
        private const string dataAdjFileName = @"..\..\InputData\wn\data.adj";
        private const string dataAdvFileName = @"..\..\InputData\wn\data.adv";
        private const string dataNounFileName = @"..\..\InputData\wn\data.noun";
        private const string dataVerbFileName = @"..\..\InputData\wn\data.verb";
        public const string QuoteReplacement = "&quot;";

        private Dictionary<string, SeekReadStream> fileStreamDict = new Dictionary<string, SeekReadStream>()
        {
            {"a", new SeekReadStream(dataAdjFileName)},
            {"r", new SeekReadStream(dataAdvFileName)},
            {"n", new SeekReadStream(dataNounFileName)},
            {"v", new SeekReadStream(dataVerbFileName)}
        };


        internal WordNet30Formatter(ITypeFormatter typeFormatter) : base("wordnet3_0", typeFormatter)
        {
        }

        protected override void Load()
        {
            var dict = new Dictionary<string, Def>();
            using (var stream = new StreamReader(indexAllFileName))
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

                foreach (var pair in dict)
                {
                    var sb = new StringBuilder();
                    string shortDefinition = null;
                    for (var j = 0; j <  pair.Value.LemmaList.Count; j++)
                    {
                        string lemma = pair.Value.LemmaList[j];
                        string[] spl = lemma.Split(new char[] { ' ' });
                        string pos = spl[0]; // 0
                        int synset_cnt = int.Parse(spl[1]); // 1
                        int p_cnt = int.Parse(spl[2]); // 2
                        int x = 2 + p_cnt + 1 + 1; // 2 - current, p_cnt skip, 1 - sense_cnt, 1 - tagsense_cnt
                        if (j > 0)
                            sb.Append(Common.NewLineDelimiter);    

                        this.AppendDecoratedPos(sb, pos);
                        sb.Append(Common.NewLineDelimiter);
                        for (var i = 1; i <= synset_cnt; i++)
                        {

                            fileStreamDict[pos].Sr.BaseStream.Seek(long.Parse(spl[x + i]), SeekOrigin.Begin);
                            fileStreamDict[pos].Sr.DiscardBufferedData();
                            Synset synset = Parse(pair.Key, fileStreamDict[pos].Sr.ReadLine());
                            shortDefinition = synset.Gloss;

                            if (i > 1)
                            {
                                sb.Append(Common.NewLineDelimiter);
                            }

                            sb.Append(i).Append(". ").Append(synset.Gloss);
                            if (synset.Synonims.Count > 0)
                            {
                                sb.Append(Common.NewLineDelimiter + this.typeFormatter.AlternateColorBegin + " (");
                                for (int k = 0; k < synset.Synonims.Count; k++)
                                {
                                    sb.Append(synset.Synonims[k]);
                                    if (k != synset.Synonims.Count - 1)
                                    {
                                        sb.Append(", ");    
                                    }
                                }

                                sb.Append(")" + this.typeFormatter.AlternateColorEnd);    

                            }

                            if (!string.IsNullOrEmpty(synset.Sentences))
                            {
                                sb.Append(Common.NewLineDelimiter + " ");
                                sb.Append(synset.Sentences);
                            }

                            sb.Append(Common.NewLineDelimiter);
                        }
                    }

                    pair.Value.Value = sb.ToString();
                    var article = new Article(pair.Key, pair.Value.Value) {ShortDefinition = shortDefinition};
                    articles.Add(article);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFileLine">Data file line as described at https://wordnet.princeton.edu/man/wndb.5WN.html (section Data File Format)</param>
        /// <returns></returns>
        public Synset Parse(string def, string dataFileLine)
        {
            string[] lineArray = dataFileLine.Split(new char[] { '|' }, 2);
            string s = lineArray[1];
            var result = new Synset();
            int posDef = s.IndexOf('"');
            if (posDef < 0)
            {
                // there are no sentences
                result.Gloss = s;
                result.Encode(this.typeFormatter);
            }
            else
            {
                result.Gloss = s.Substring(0, posDef);
                result.Gloss = result.Gloss.TrimEnd(';', ' ');

                if (posDef < s.Length)
                {
                    result.Sentences = s.Substring(posDef);
                }

                result.Encode(this.typeFormatter);
                result.Sentences = ItalianQuotes(result.Sentences);
            }

            result.FillSynonims(def, lineArray[0]);
            return result;
        }

      

        public string ItalianQuotes(string s)
        {
            string original = s;
            int i = 0;
            int count = 0;
            while (i < s.Length)
            {
                int pos = s.IndexOf(QuoteReplacement, i, StringComparison.OrdinalIgnoreCase);
                if (pos >= 0)
                {
                    s = s.Remove(pos, QuoteReplacement.Length);
                    string tag = count % 2 == 0 ? "• " + this.typeFormatter.AlternateColorBegin : this.typeFormatter.AlternateColorEnd;
                    s = s.Insert(pos, tag);
                    i = pos + tag.Length;
                    count++;

                    if (count % 2 == 0)
                    {
                        if (i < s.Length && s[i] == ';' && count % 2 == 0)
                        {
                            // end of example, let's make new line
                            s = s.Remove(i, 1);
                            string newLine = Common.NewLineDelimiter.ToString(CultureInfo.InvariantCulture);
                            s = s.Insert(i, newLine);
                            i += newLine.Length;
                        }
                    }
                }
                else
                {
                    i++;
                }
            }

            // checking that count is even, otherwise we created invalid xml.
            return count % 2 == 0 ?  s : original;
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

        private void AppendDecoratedPos(StringBuilder result, string pos)
        {
            result.Append(this.typeFormatter.AccentColorBegin);
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

            result.Append(this.typeFormatter.AlternateColorEnd);
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

        public class Synset
        {
            public string Gloss { get; set; }

            public string Sentences { get; set; }

            public List<string> Synonims { get; private set; }

            public void Encode(ITypeFormatter formatter)
            {
                this.Gloss = formatter.Escape(Gloss);
                this.Sentences = formatter.Escape(this.Sentences);
            }

            public void FillSynonims(string def, string m)
            {
                string[] a = m.Split(' ');
                int synCount = Convert.ToInt32(a[3], 16) - 1;
                this.Synonims = new List<string>();
                int start = 6;
                for (int i = 0; i < synCount; i++)
                {
                    var cand = a[start].Replace('_', ' ');
                    if (cand != def)
                    {
                        Synonims.Add(cand);
                    }

                    start += 2;
                }
            }
        }
    }
}
