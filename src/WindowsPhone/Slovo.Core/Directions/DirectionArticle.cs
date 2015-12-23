namespace Slovo.Core.Directions
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [DataContract]
    public class DirectionArticle
    {
        private const long Mulitplier = 10000000000;

        internal const int MaxVocabulariesInDirection = 100;

        public DirectionArticle()
        {
        }

        public DirectionArticle(string sense)
        {
            this.DefinitionOffsets = new Dictionary<int, int>();
            this.Sense = sense;
            this.OffsetForTwoNumbers = -1;
        }

        internal DirectionArticle(string sense, int vocabularyId, int definitionOffset)
            : this(sense)
        {
            this.DefinitionOffsets.Add(vocabularyId, definitionOffset);
        }

        /// <summary>
        /// Gets or sets the sense of the article
        /// </summary>
        [DataMember]
        public string Sense { get; set; }

        /// <summary>
        /// Gets or sets direction id to which this article belongs
        /// </summary>
        [DataMember]
        public int DirectionId { get; set; }

        /// <summary>
        /// Gets or sets short definition for the article
        /// </summary>
        [DataMember]
        public string ShortDefinition { get; set; }

        /// <summary>
        /// Gets or sets the index in direction for the current article.
        /// </summary>
        [XmlIgnore()]
        public int WordIndex { get; set; }

        /// <summary>
        /// Mapping list between vocabulary id and definition offset in the vocabulary
        /// </summary>
        [DataMember(Name = "DefinitionOffsets_3_0_0_0")]
        public Dictionary<int, int> DefinitionOffsets { get; set;}

        public override bool Equals(object obj)
        {
            //obj is null 
            if (obj == null)
            {
                return false;
            }

            DirectionArticle other = obj as DirectionArticle;
            if (other != null)
            {
                return this.Sense == other.Sense;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = -1;
            if (!string.IsNullOrEmpty(this.Sense))
            {
                result = this.Sense.GetHashCode();
            }

            return result;
        }


        internal long OffsetForTwoNumbers { private get; set; }

        internal int GetDefinitionOffsetByVocabularyId(int vocabularyId)
        {
            return this.DefinitionOffsets[vocabularyId] * MaxVocabulariesInDirection + (int)vocabularyId;
        }
        
        /// <summary>
        /// Updates offsets in case there are only two vocabularies in the current direction for Direction article
        /// </summary>
        /// <param name="vocabularyId1">int of the vocabulary that is stored first in direction file</param>
        /// <param name="vocabularyId2">int of the vocabulary that is stored second in direction file</param>
        internal void UpdateDefinitionOffsetForTwoVocabularies(int vocabularyId1, int vocabularyId2)
        {
            if (this.OffsetForTwoNumbers != -1)
            {
                int first;
                int second;
                DeserializeNumbers(this.OffsetForTwoNumbers, out first, out second);
                int noOffset = Common.NoOffset;
                if (first != noOffset)
                {
                    this.DefinitionOffsets.Add(vocabularyId1, first);
                }

                if (second != noOffset)
                {
                    this.DefinitionOffsets.Add(vocabularyId2, second);
                }

                this.OffsetForTwoNumbers = -1;
            }
        }

        internal static long SerializeNumbers(int firstNumber, int secondNumber)
        {
            return (long)firstNumber * Mulitplier + secondNumber;
        }

        internal static void DeserializeNumbers(long number, out int firstNumber, out int secondNumber)
        {
            firstNumber = (int)(number / Mulitplier);
            secondNumber = (int)(number % Mulitplier);
        }
    }
}
