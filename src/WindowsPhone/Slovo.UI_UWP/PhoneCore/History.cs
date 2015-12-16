namespace Slovo.Core
{
   using Slovo.Core.Directions;
   using System.Collections.Generic;
   using System.IO.IsolatedStorage;

   internal class History : IListIterator<Directions.DirectionArticle>
   {
      private const int Limit = 150;
      private const int RemoveCount = 50;
      private const string HistoryKey = "HistoryKey";
      private List<Slovo.Core.Directions.DirectionArticle> all;
      private bool isChanged = false;
      private int currentIndex;

      internal List<Slovo.Core.Directions.DirectionArticle> Items
      {
         get
         {
            if ( this.all == null )
            {
               Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
               this.all = settings.Values.ContainsKey(HistoryKey) ? (List<Slovo.Core.Directions.DirectionArticle>)settings.Values[HistoryKey] : new List<Slovo.Core.Directions.DirectionArticle>();
            }
            return this.all;
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
            if ( this.currentIndex < this.Items.Count - 1 )
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
            if ( this.currentIndex > 0 )
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
         if ( index == -1 )
         {
            if ( this.Items.Count == Limit )
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
         if ( index != -1 )
         {
            this.Items.RemoveAt(index);
         }
         isChanged = true;
      }

      internal void Save()
      {
         Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
         if ( settings.Values.ContainsKey(HistoryKey) )
         {
            settings.Values[HistoryKey] = this.Items;
         }
         else
         {
            settings.Values.Add(HistoryKey, this.Items);
         }
      }
   }
}