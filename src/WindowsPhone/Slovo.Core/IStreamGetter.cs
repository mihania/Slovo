
namespace Slovo.Core
{
    using System;
    using System.IO;
    using System.Windows;

    public interface IStreamGetter
    {
        Stream GetStream(string fileName);
        
        Stream CreateStream(string fileName);

        bool FileExists(string fileName);
    }
}
