namespace Slovo.Core
{
    using Slovo.Core.Config;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using System;
    using System.Collections.Generic;

    internal class Manager<T> where T : IStreamGetter, new()
    {
        private static Manager<T> _instance;
        private readonly T streamGetter = new T();
        private History<T> history = new History<T>();

        private Manager()
        {
            this.Configuration = Configuration<T>.LoadConfiguration();
        }

        internal static Manager<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Manager<T>();
                }
                return _instance;
            }
        }

        public Configuration<T> Configuration { get; set; }

        internal Direction<T> CurrentDirection { get; set; }

        internal History<T> History
        {
            get
            {
                return this.history;
            }
        }

        internal string LastSearchedWord
        {
            get
            {
                return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["LastSearchedWord"];
            }

            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["LastSearchedWord"] = value;
            }
        }

        internal LoadingState CurrentLoadingState
        {
            get
            {
                LoadingState result = LoadingState.NotLoaded;
                if (this.CurrentDirection != null)
                {
                    result = this.CurrentDirection.LoadingState;
                }
                return result;
            }
        }

        internal Vocabulary<T> GetVocabulary(int voc)
        {
            Vocabulary<T> result = null;
            foreach (var direction in this.Configuration.Directions)
            {
                foreach (var vocabulary in direction.Vocabularies)
                {
                    if (vocabulary.VocabularyId == voc)
                    {
                        result = vocabulary;
                        break;
                    }
                }
            }
            if (result == null)
            {
                throw new ArgumentOutOfRangeException(voc.ToString());
            }
            return result;
        }

        internal Direction<T> GetDirection(int directionId, LoadingState loadingState)
        {
            var result = this.Configuration.Directions.GetDirectionById(directionId);
            if (result.LoadingState != LoadingState.Loaded)
            {
                this.Configuration.Directions.GetDirectionById(directionId).Deserialize(loadingState);
            }
            return result;
        }

        /// <summary>
        /// Update directions
        /// </summary>
        /// <param name="newDirections">New direction</param>
        /// <returns>True if update was required, otherwise false</returns>
        internal EqualStatus UpdateDirections(DirectionList<T> newDirections)
        {
            var result = EqualStatus.Equal;
            // looping through keys because inside loop dictionary is modified
            var directionIdList = new List<int>();
            foreach (var direction in this.Configuration.Directions)
            {
                directionIdList.Add(direction.Id);
            }
            foreach (var directionId in directionIdList)
            {
                var oldDirection = this.Configuration.Directions.GetDirectionById(directionId);
                var newDirection = newDirections[directionId];
                if (newDirection != null)
                {
                    var equalStatus = newDirection.Equals(oldDirection);
                    if (equalStatus != EqualStatus.Equal)
                    {
                        if (equalStatus == EqualStatus.NotEqual)
                        {
                            result = equalStatus;
                            newDirection.Serialize();
                        }
                        else if (equalStatus == EqualStatus.SameEnabledVocabularies)
                        {
                            // check that all previous vocabularies are equal
                            if (result == EqualStatus.Equal)
                            {
                                result = equalStatus;
                            }
                            // initialy IndexInDirectionFile is set manually in Configuration.xml
                            // if user only changes the vocabularies order in direction, we need to update it in Confiration.xml
                            foreach (var newVocabulary in newDirection.Vocabularies)
                            {
                                var vocabularyInOldDirection = oldDirection.GetVocabularyByVocabularyId(newVocabulary.VocabularyId);
                                if (vocabularyInOldDirection == null)
                                {
                                    throw new VocabularyNotFoundException("Vocabulary must exist in old direction as only order of vocabularies changed.");
                                }
                                var oldIndexInDirection = vocabularyInOldDirection.IndexInDirectionFile;
                                newVocabulary.IndexInDirectionFile = oldIndexInDirection;
                            }
                        }
                        this.Configuration.Directions[directionId] = newDirection;
                    }
                }
            }
            return result;
        }
    }
}