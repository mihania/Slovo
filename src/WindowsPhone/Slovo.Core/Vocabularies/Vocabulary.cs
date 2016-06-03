namespace Slovo.Core.Vocabularies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using Slovo.Core.Config;
    using Microsoft.HockeyApp;

    public class Vocabulary<T> : SenseList<Article> where T : IStreamGetter, new()
    {
        private readonly T streamGetter = new T();

        public const string IndexFileExtension = ".lst";

        public const string DataFileExtension = ".def";

        [XmlIgnore]
        public string SenseFile { get { return "Data/" + Name + IndexFileExtension; } }

        [XmlIgnore]
        public string DefFile { get { return "Data/" + Name + DataFileExtension; } }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("IndexInDirectionFile")]
        public int IndexInDirectionFile { get; set; }

        /// <summary>
        /// Gets or sets the value whether vocabulary words will be added to direction
        /// </summary>
        [XmlAttribute("IsEnabled")]
        public bool IsEnabled { get; set; }

        [XmlAttribute("Id")]
        public int VocabularyId { get; set; }

        [XmlArray("PivotHeaders")]
        [XmlArrayItem("Resource")]
        public ResourceList PivotHeaders { get; set; } 

        /// <summary>
        /// Gets the tab name shown on the pivot control
        /// </summary>
        public string PivotHeader { get { return this.PivotHeaders.GetName(); } }

        internal List<int> IndexesList { get; set; }

        internal int Count { private set; get; }

        public override Article GetArticle(int offset)
        {
            HockeyClient.Current.TrackEvent("Vocabulary.GetArticle");
            // ToDo: Think on opening file only once
            using (Stream stream = this.streamGetter.GetStream(this.DefFile)) 
            {
                using (BinaryReader sr = new BinaryReader(stream, Common.VocabularyEncoding))
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    string shortDefinition = sr.ReadString();
                    string definition = sr.ReadString();
                    return new Article(definition, this.VocabularyId, offset);
                }
            }
        }

        internal string GetShortDefinition(int offset)
        {
            string result = string.Empty;

            using (Stream stream = this.streamGetter.GetStream(this.DefFile))
            {
                using (BinaryReader sr = new BinaryReader(stream, Common.VocabularyEncoding))
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    result = sr.ReadString();
                }
            }

            return result;
        }

        public override void Deserialize(LoadingState loading)
        {
            this.Load();
        }

        internal void Load()
        {
            this.List = new List<string>();
            this.IndexesList = new List<int>();
            using (var stream = this.streamGetter.GetStream(this.SenseFile))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    using (BinaryReader sr = new BinaryReader(memoryStream, Common.VocabularyEncoding))
                    {
                        this.Count = sr.ReadInt32();
                        int i = 0;
                        while (i < this.Count)
                        {
                            List.Add(sr.ReadString());
                            IndexesList.Add(sr.ReadInt32());
                            i++;
                        }
                    }
                }
            }

            this.LoadingState = LoadingState.Loaded;
        }

        internal Vocabulary<T> Clone()
        {
            var result = new Vocabulary<T>();
            result.IsEnabled = this.IsEnabled;
            result.IndexesList = this.IndexesList;
            result.Count = this.Count;
            result.PivotHeaders = this.PivotHeaders.Clone();
            result.Name = this.Name;
            result.VocabularyId = this.VocabularyId;
            result.IndexInDirectionFile = this.IndexInDirectionFile;
            return result;
        }
    }
}