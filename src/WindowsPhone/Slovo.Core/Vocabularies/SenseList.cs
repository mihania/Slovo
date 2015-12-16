namespace Slovo.Core.Vocabularies
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public abstract class SenseList<T> : IListIterator<T>
    {
        protected SenseList()
        {
            this.LoadingState = LoadingState.NotLoaded;
        }

        [XmlIgnore]
        public List<string> List { get; set; }

        #region IListIterator

        [XmlIgnore]
        public int Cursor { get; set; }

        [XmlIgnore]
        public bool HasNext { get { return this.HasPrevious; } }

        [XmlIgnore]
        public bool HasPrevious { get { return this.List.Count > 1; } }

        [XmlIgnore]
        public T Next
        {
            get
            {
                T result = default(T);

                if (this.Cursor < this.List.Count - 1)
                {
                    this.Cursor++;
                }
                else
                {
                    this.Cursor = 0;
                }

                result = this.GetArticle(this.Cursor);

                return result;
            }
        }

        [XmlIgnore]
        public T Previous
        {
            get
            {
                T result = default(T);

                if (this.Cursor > 0)
                {
                    this.Cursor--;
                }
                else
                {
                    this.Cursor = this.List.Count - 1;
                }

                result = this.GetArticle(this.Cursor);

                return result;
            }
        }

        #endregion

        [XmlIgnore]
        public string this[int i]
        {
            get { return List[i]; }
        }

        [XmlIgnore]
        public bool IsWordFound
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets or sets vocabulary loading state.
        /// ToDo: Make private set
        /// </summary>
        [XmlIgnore]
        public LoadingState LoadingState { get; set; }

        /// <summary>
        /// Sets the cursor to a position relevant to the text
        /// </summary>
        /// <param name="text">Search text</param>
        /// <returns>Cursor position</returns>
        public int SetPosition(string text)
        {
            // it was changed from StringComparer.InvariantCultureIngoreCase
            if (this.List == null)
            {
                // this.List is not loaded still.
                this.IsWordFound = false;
            }
            else
            {
                this.Cursor = this.List.BinarySearch(text, Common.StringComparer);
                if (this.Cursor < 0)
                {
                    this.IsWordFound = false;
                    this.Cursor = ~this.Cursor;
                }
                else
                {
                    this.IsWordFound = true;
                }
            }

            return this.Cursor;
        }

        public abstract T GetArticle(int numberInList);

        public abstract void Deserialize(LoadingState loading);
    }
}
