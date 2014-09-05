namespace Slovo.Core
{
    using System.Collections.Generic;

    public interface IListIterator<T>
    {
        bool HasNext { get; }

        bool HasPrevious { get; }

        T Next { get; }

        T Previous { get; }
    }
}
