namespace Slovo.Core.Vocabularies
{
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.Text;
    using System;
    using Slovo.Core.Vocabularies;
    using System.Collections.ObjectModel;

    [DataContract(Name = "Article")]
    public class Article : IEquatable<Article>
    {
        public Article(string definition, int vocabularyId, int offset)
        {
            this.Definition = definition;
            this.VocabularyId = vocabularyId;
            this.Offset = offset;
        }

        [XmlIgnore()]
        public string Definition { get; set; }

        
        [DataMember]
        public int VocabularyId { get; set; }

        /// <summary>
        /// Offset for the defition in current article.
        /// <remarks>Offset is stored in application storage for history instead of the definition in order o minimize memory size consumption</remarks>
        /// </summary>
        [DataMember]
        public int Offset {get;set;}

        public bool Equals(Article other)
        {
            bool result = false;
            if (other != null) 
            {
                result = this.VocabularyId == other.VocabularyId;
            }
            return result;
        }
    }
}
