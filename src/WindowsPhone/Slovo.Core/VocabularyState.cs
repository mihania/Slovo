namespace Slovo.Core
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Input;

    public enum LoadingState
    {
        NotLoaded = 0,
        HeadersLoading = 1,
        HeadersLoaded = 2,
        Loading = 3,
        Loaded = 4
    }
}
