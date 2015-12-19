namespace Slovo.Core
{
    using Slovo.Core.Directions;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using Slovo.UI.PhoneCore;
    using System.Threading.Tasks;

    internal class History<T> : IListIterator<Directions.DirectionArticle> where T : IStreamGetter, new()
    {
        private const int Limit = 150;
        private const int RemoveCount = 50;
        private const string HistoryFileName = "History.json";
        private bool isChanged = false;
        private int currentIndex;
        private bool loadedFromStorage = false;

        internal History()
        {
            this.Items = new List<Slovo.Core.Directions.DirectionArticle>();
        }

        internal List<Slovo.Core.Directions.DirectionArticle> Items { get; private set; }


        internal async Task EnsurePersistenceHistoryLoaded()
        {
            if (!loadedFromStorage)
            {
                var persistenceHistory = await XmlIO.ReadObjectFromXmlFileAsync<List<Slovo.Core.Directions.DirectionArticle>>(HistoryFileName);
                this.Items.AddRange(persistenceHistory);
                loadedFromStorage = true;
            }
        }

        internal bool IsChanged
        {
            get
            {
                return isChanged;
            }
        }

        internal Directions.DirectionArticle Last
        {
            get
            {
                return this.Items.Count == 0 ? null : this.Items[0];
            }
        }

        #region ListIterator
        public bool HasNext
        {
            get
            {
                return HasPrevious;
            }
        }

        public bool HasPrevious
        {
            get
            {
                return this.Items.Count > 1;
            }
        }

        public Directions.DirectionArticle Next
        {
            get
            {
                if (this.currentIndex < this.Items.Count - 1)
                {
                    this.currentIndex++;
                }
                else
                {
                    this.currentIndex = 0;
                }
                return this.Items[this.currentIndex];
            }
        }

        public DirectionArticle Previous
        {
            get
            {
                if (this.currentIndex > 0)
                {
                    this.currentIndex--;
                }
                else
                {
                    this.currentIndex = this.Items.Count - 1;
                }
                return this.Items[this.currentIndex];
            }
        }
        #endregion

        internal int CurrentIndex
        {
            set
            {
                this.currentIndex = value;
            }
        }

        internal void Add(Slovo.Core.Directions.DirectionArticle item)
        {
            int index = this.Items.IndexOf(item);
            if (index == -1)
            {
                if (this.Items.Count == Limit)
                {
                    this.Items.RemoveRange(Limit - RemoveCount, RemoveCount);
                }
            }
            else
            {
                this.Items.RemoveAt(index);
            }
            this.Items.Insert(0, item);
            isChanged = true;
        }

        internal void Remove(Slovo.Core.Directions.DirectionArticle item)
        {
            int index = this.Items.IndexOf(item);
            if (index != -1)
            {
                this.Items.RemoveAt(index);
            }
            isChanged = true;
        }

        internal async Task Save()
        {
            await EnsurePersistenceHistoryLoaded();
            await XmlIO.SaveObjectToXml(this.Items, HistoryFileName);
        }
    }
}