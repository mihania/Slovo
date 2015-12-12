namespace Slovo.Core
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IStreamGetter
    {
        Stream GetStream(string fileName);
        
        Stream CreateStream(string fileName);

        bool FileExists(string fileName);
    }
}