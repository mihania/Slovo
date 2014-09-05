namespace Slovo.Core.Config
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using System.IO;
    using System.Collections.ObjectModel;

    [XmlRoot("Configuration")]
    public class Configuration<T, E>
        where T : IStreamGetter, new()
        where E : ICollection<Vocabulary<T>>, IList<Vocabulary<T>>, new()
    {
        private const string ConfigurationFilePath = @"Data/Configuration.xml";
        private T streamGetter = new T();

        [XmlArray("Directions")]
        [XmlArrayItem("Direction")]
        public DirectionList<T, E> Directions { get; set; }

        public static Configuration<T, E> LoadConfiguration()
        {
            using (var stream = new T().GetStream(ConfigurationFilePath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    XmlSerializer serializer = new XmlSerializer(typeof(Configuration<T, E>));
                    memoryStream.Position = 0;
                    var result = (Configuration<T, E>) serializer.Deserialize(memoryStream);
                    return result;
                }
            }
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration<T, E>));
            using (Stream s = streamGetter.CreateStream(ConfigurationFilePath))
            {
                serializer.Serialize(s, this);
            }
        }
    }

    public class DirectionList<T, E> : List<Direction<T, E>>
       where T : IStreamGetter, new()
       where E : ICollection<Vocabulary<T>>, IList<Vocabulary<T>>, new()
    {
        public Direction<T, E> GetDirectionById(int directionId)
        {
            Direction<T, E> result = null;
            foreach (var direction in this)
            {
                if (direction.Id == directionId)
                {
                    result = direction;
                    break;
                }
            }

            return result;
        }

        public DirectionList<T, E> Clone()
        {
            var result = new DirectionList<T, E>();
            foreach (var element in this)
            {
                result.Add(element.Clone());
            }

            return result;
        }

        public bool Equals(DirectionList<T, E> other)
        {
            var result = true;
            foreach (var otherElem in other)
            {
                var thisElem = this.GetDirectionById(otherElem.Id);
                if (thisElem == null || !thisElem.Equals(other))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }

    
    public class ResourceList : List<Resource> 
    {
        public string GetName()
        {
            string result = null;
            var currentTwoLetterISOLanguageName = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var defaultTwoLetterISOLanguageName = "en";
            string defaultResult = null;
            foreach (var resource in this)
            {
                if (resource.LanguageCode == defaultTwoLetterISOLanguageName) 
                {
                    defaultResult = resource.Value;
                }
                else if (resource.LanguageCode == currentTwoLetterISOLanguageName)
                {
                    result = resource.Value;
                }
            }

            if (result == null)
            {
                result = defaultResult;
            }

            return result;

        }

        public ResourceList Clone()
        {
            // this is not changing ever. We can just return the reference
            return this;
        }
    }

    public partial class Resource
    {
        [XmlAttribute]
        public string LanguageCode { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}