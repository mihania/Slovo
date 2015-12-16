namespace Slovo.UI
{
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;




    public struct Word
    {

        public string Name { get; set; }

    }

    internal class VirtualWordList : IList<string>, IList
    {
        private IList<string> _originalList;

        internal VirtualWordList(IList<string> originalList)
        {
            _originalList = originalList;
        }

        /// <summary>
        /// Return the total number of items in your list.
        /// </summary>
        public int Count
        {
            get
            {
                return _originalList.Count;
            }
        }

        string IList<string>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        object IList.this[int index]
        {
            get
            {
                // here is where the magic happens, create/load your data on the fly.
                //http://shawnoster.com/blog/post/Improving-ListBox-Performance-in-Silverlight-for-Windows-Phone-7-Data-Virtualization.aspx
                Debug.WriteLine("Requsted item " + index.ToString());
                return new Word()
                {
                    Name = _originalList[index]
                };
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int IndexOf(string item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object item)
        {
            // this method is invoked from ListView.ScrollIntoView.
            return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance.CurrentDirection.Cursor;
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public virtual void CopyTo(Array ary, int index)
        {
        }

        public void Clear()
        {
        }

        public void RemoveAt(string at)
        {
        }

        public void RemoveAt(int at)
        {
        }

        public void CopyTo(string[] s, int at)
        {
        }

        public int Remove(string[] s, int at)
        {
            throw new NotImplementedException();
        }

        public void Remove(string[] s)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string s)
        {
            throw new NotImplementedException();
        }

        public void Remove(object s)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Insert(int a, string b)
        {
        }

        public void Insert(int a, object b)
        {
        }

        public void Add(string a)
        {
        }

        public int Add(object a)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string a)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object a)
        {
            throw new NotImplementedException();
        }
    }
}