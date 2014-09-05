namespace Slovo.Core.Directions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Slovo.Core.Vocabularies;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using Slovo.Core.Config;
    using System.Globalization;

    /// <summary>
    /// The set of vocabularies with the same source language
    /// </summary>
    public class Direction<T, E> : SenseList<DirectionArticle>
        where E : ICollection<Vocabulary<T>>, IList<Vocabulary<T>>, new()
        where T : IStreamGetter, new() 
    {
        
        private readonly T streamGetter = new T();
        private long[] indexList;
        private int[] indexList1;
        private object deserializeLockObject = new object();

        /// <summary>
        /// Determines headers start position in direction stream
        /// </summary>
        private long headerStart = -1;

        public Direction()
        {
        }

        public Direction(IEnumerable<Vocabulary<T>> vocabularies)
        {
            this.Vocabularies = new E();
            foreach (var voc in vocabularies)
            {
                this.Vocabularies.Add(voc);
            }
        }

        /// <summary>
        /// Gets or sets vocabularies in the order they are represented on the pivot tabs
        /// </summary>
        [XmlArray("Vocabularies")]
        [XmlArrayItem("Vocabulary")]
        public E Vocabularies { get; set; }

        /// <summary>
        /// Gets or sets direction id
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute]
        public string SourceLanguageCode { get; set; }

        /// <summary>
        /// Determines a subset of words shown for customers in trial mode
        /// </summary>
        [Obsolete("Not used anymore as application does not support trial mode now")]
        [XmlIgnore]
        public string TrialLimitLetter { get; set; }

        /// <summary>
        /// Gets file name to save direction to the file system
        /// </summary>
        [XmlAttribute]
        public string DataFileName { get; set; }

        [XmlArray("Names")]
        [XmlArrayItem("Resource")]
        public ResourceList Names { get; set; }

        /// <summary>
        /// Gets the direction name shown to the user
        /// </summary>
        [XmlIgnore]
        public string Name 
        { 
            get 
            {
                return this.Names.GetName();
            } 
        }

        [XmlIgnore]
        internal E EnabledVocabularies
        {
            get
            {
                E result = new E();
                foreach (var voc in this.Vocabularies)
                {
                    if (voc.IsEnabled)
                    {
                        result.Add(voc);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Serialize to disk
        /// </summary>
        public void Serialize()
        {
            this.UpdateVocabularyIndexesInDirectionFile();
            var enabledVocabularies = this.EnabledVocabularies;
            var enabledVocabulariesCount = enabledVocabularies.Count;
            using (Stream s = streamGetter.CreateStream(DataFileName))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    var directionArticleList = this.GetDirectionArticleList();
                    bw.Write(directionArticleList.Count);
                    var twoVocabulariesOffsets = new int[enabledVocabulariesCount];

                    if (enabledVocabulariesCount > 0)
                    {
                        int firstVocabularyId = enabledVocabularies[0].VocabularyId;

                        foreach (var directionArticle in directionArticleList)
                        {
                            bw.Write(directionArticle.Sense);
                            if (enabledVocabulariesCount == 1)
                            {
                                // Performance optimization: Write only 1 integer
                                bw.Write(directionArticle.DefinitionOffsets[firstVocabularyId]);
                            }
                            else if (enabledVocabulariesCount == 2)
                            {
                                // Performance optimization: Write only 2 integers.
                                for (int i = 0; i < enabledVocabulariesCount; i++)
                                {
                                    int offset;
                                    int vocabularyId = GetVocabularyIdByIndexInDirectionFile(i);
                                    if (directionArticle.DefinitionOffsets.TryGetValue(vocabularyId, out offset))
                                    {
                                        twoVocabulariesOffsets[i] = offset;
                                    }
                                    else
                                    {
                                        twoVocabulariesOffsets[i] = Common.NoOffset;
                                    }
                                }

                                long oneNumber = DirectionArticle.SerializeNumbers(twoVocabulariesOffsets[0], twoVocabulariesOffsets[1]);
                                bw.Write(oneNumber);
                            }
                            else
                            {
                                bw.Write(directionArticle.DefinitionOffsets.Count);
                                foreach (var keyPair in directionArticle.DefinitionOffsets)
                                {
                                    bw.Write(directionArticle.GetDefinitionOffsetByVocabularyId(keyPair.Key));
                                }
                            }
                        } // for
                    }
                }
            }
        }

        public override void Deserialize(LoadingState destinationLoadingState)
        {
            // prevet IsolatedStorageFileException on that occurs on opening files from two different threads
            lock (deserializeLockObject)
            {
                if (this.LoadingState < LoadingState.HeadersLoaded)
                {
                    this.LoadingState = LoadingState.HeadersLoading;
                    this.List = new List<string>();
                    this.LoadingState = LoadingState.HeadersLoaded;
                }

                if (destinationLoadingState == LoadingState.Loaded && this.LoadingState < LoadingState.Loaded)
                {
                    this.LoadingState = LoadingState.Loading;
                    this.DeserializeData();
                    this.LoadingState = LoadingState.Loaded;
                }
            }
        }
        
        public override DirectionArticle GetArticle(int wordIndex)
        {
            var da = new DirectionArticle(this[wordIndex]);
            da.DirectionId = this.Id;
            da.WordIndex = wordIndex;
            var enabledVocabulariesCount = this.EnabledVocabularies.Count;
            if (enabledVocabulariesCount == 1)
            {
                da.DefinitionOffsets.Add(this.EnabledVocabularies[0].VocabularyId, indexList1[wordIndex]);
            }
            else if (enabledVocabulariesCount == 2)
            {
                da.OffsetForTwoNumbers = indexList[wordIndex];
                var firstIndexInDirectionFile = this.Vocabularies[0].IndexInDirectionFile;
                var secondIndexInDirectionFile = this.Vocabularies[1].IndexInDirectionFile;
                da.UpdateDefinitionOffsetForTwoVocabularies(this.GetVocabularyIdByIndexInDirectionFile(firstIndexInDirectionFile), this.GetVocabularyIdByIndexInDirectionFile(secondIndexInDirectionFile));
            }

            if (da.DefinitionOffsets.Count > 0)
            {
                var firstVocabulary = this.GetFirstVocabulary(da);
                da.ShortDefinition = firstVocabulary.GetShortDefinition(da.DefinitionOffsets[firstVocabulary.VocabularyId]);
            }

            return da;
        }

        public bool IsSupportedInTrial(int index)
        {
            return this.List[index].Length == 0 || TrialLimitLetter[0] == this.List[index][0];
        }

        /// <summary>
        /// Merges vocabularies in direction
        /// </summary>
        internal List<DirectionArticle> GetDirectionArticleList()
        {
            var enabledVocabularies = this.EnabledVocabularies;
            int vocabulariesCount = enabledVocabularies.Count;
            Vocabulary<T>[] vocArray = new Vocabulary<T>[vocabulariesCount];
            int[] positions = new int[vocabulariesCount];

            for (var i = 0; i < vocabulariesCount; i++)
            {
                vocArray[i] = enabledVocabularies[i];
                if (vocArray[i].LoadingState != LoadingState.Loaded)
                {
                    vocArray[i].Load();
                }

                positions[i] = 0;
            }

            var directionArticleList = new List<DirectionArticle>();
            while (true)
            {
                int minIndex = -1;
                for (int i = 0; i < vocabulariesCount; i++)
                {
                    if (positions[i] < vocArray[i].Count)
                    {
                        if (minIndex == -1 || String.Compare(vocArray[i][positions[i]], vocArray[minIndex][positions[minIndex]]) < 0)
                        {
                            minIndex = i;
                        }
                    }
                }

                if (minIndex == -1)
                {
                    break;
                }


                string minSense = vocArray[minIndex][positions[minIndex]];
                int minOffset = vocArray[minIndex].IndexesList[positions[minIndex]];

                if (directionArticleList.Count > 0 && directionArticleList[directionArticleList.Count - 1].Sense == minSense)
                {
                    directionArticleList[directionArticleList.Count - 1].DefinitionOffsets.Add(vocArray[minIndex].VocabularyId, minOffset);
                }
                else
                {
                    directionArticleList.Add(new DirectionArticle(minSense, vocArray[minIndex].VocabularyId, minOffset));
                }

                positions[minIndex]++;
            }

            return directionArticleList;
        }

        internal Direction<T, E> Clone()
        {
            var result = new Direction<T, E>();
            if (this.Vocabularies != null)
            {
                result.Vocabularies = new E();
                foreach (var voc in this.Vocabularies)
                {
                    result.Vocabularies.Add(voc.Clone());
                }
            }

            result.indexList = this.indexList;
            result.indexList1 = this.indexList1;
            result.headerStart = this.headerStart;
            result.Id = this.Id;
            result.SourceLanguageCode = this.SourceLanguageCode;
            result.TrialLimitLetter = this.TrialLimitLetter;
            result.DataFileName = this.DataFileName;
            result.Names = this.Names.Clone();

            return result;
        }

        public EqualStatus Equals(Direction<T, E> other)
        {
            var result = EqualStatus.Equal;
            if (other == null)
            {
                result = EqualStatus.NotEqual;
            }
            else if (this != other)
            {
                if (this.Vocabularies.Count == other.Vocabularies.Count)
                {
                    for (int i = 0; i < this.Vocabularies.Count; i++) 
                    {
                        var contains = false;
                        for (int j = 0; j < other.Vocabularies.Count; j++) 
                        {
                            if (this.Vocabularies[i].VocabularyId == other.Vocabularies[j].VocabularyId)
                            {
                                if (this.Vocabularies[i].IsEnabled == other.Vocabularies[j].IsEnabled) 
                                {
                                    contains = true;
                                    if (i != j)
                                    {
                                        // different order
                                        result = EqualStatus.SameEnabledVocabularies;
                                    }
                                }
                            }
                        }

                        if (!contains) {
                            result = EqualStatus.NotEqual;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vocabularyId"></param>
        /// <returns>Vocabulary if exists, otherwise null</returns>
        public Vocabulary<T> GetVocabularyByVocabularyId(int vocabularyId)
        {
            Vocabulary<T> result = null;
            foreach (var voc in this.Vocabularies)
            {
                if (voc.VocabularyId == vocabularyId)
                {
                    result = voc;
                }
            }

            return result;
        }

        private void UpdateVocabularyIndexesInDirectionFile()
        {
            for (int i = 0; i < this.EnabledVocabularies.Count; i++)
            {
                this.EnabledVocabularies[i].IndexInDirectionFile = i;
            }
        }

        private void DeserializeData()
        {
            var vocNamesCount = this.EnabledVocabularies.Count;
            using (Stream s = streamGetter.GetStream(DataFileName))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    s.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    using (BinaryReader br = new BinaryReader(memoryStream))
                    {
                        int count = br.ReadInt32();
                        if (vocNamesCount == 1)
                        {
                            this.indexList1 = new int[count];
                        }
                        else if (vocNamesCount == 2)
                        {
                            this.indexList = new long[count];
                        }

                        int i = 0;
                        while (i < count)
                        {
                            string sense = br.ReadString();
                            if (vocNamesCount == 1)
                            {
                                int offset = br.ReadInt32();
                                this.indexList1[i] = offset;
                            }
                            else if (vocNamesCount == 2)
                            {
                                this.indexList[i] = br.ReadInt64();
                            }
                            else
                            {
                                int offsetsCount = br.ReadInt32();
                                for (var j = 0; j < offsetsCount; j++)
                                {
                                    // da.DefinitionOffsets.Add(vocNames[j], br.ReadInt32());
                                }
                            }

                            this.List.Add(sense);
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets first vocabulary ordered by tab order where direction article contains an article
        /// </summary>
        /// <param name="directionArticle"></param>
        /// <returns></returns>
        private Vocabulary<T> GetFirstVocabulary(DirectionArticle directionArticle)
        {
            Vocabulary<T> result = null;
            foreach (var voc in this.Vocabularies)
            {
                if (directionArticle.DefinitionOffsets.ContainsKey(voc.VocabularyId))
                {
                    result = voc;
                    break;
                }
            }

            return result;
        }

        private int GetVocabularyIdByIndexInDirectionFile(int indexInDirectionFile)
        {
            return this.GetVocabularyByIndexInDirectionFile(indexInDirectionFile).VocabularyId;
        }

        private Vocabulary<T> GetVocabularyByIndexInDirectionFile(int indexInDirectionFile)
        {
            Vocabulary<T> result = null;
            bool found = false;
            for (int i = 0; i < this.Vocabularies.Count; i++)
            {
                if (i == indexInDirectionFile)
                {
                    found = true;
                    result = this.Vocabularies[i];
                }
            }

            if (!found)
            {
                throw new ArgumentOutOfRangeException("indexInDirectionFile");
            }

            return result;
        }
    }
}
