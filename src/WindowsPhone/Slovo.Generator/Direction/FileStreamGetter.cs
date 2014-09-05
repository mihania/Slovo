
namespace Slovo.Generator.Direction
{
    using System.IO;
    using Slovo.Core;

    /// <summary>
    /// File Stream Getter
    /// </summary>
    /// <remarks>
    /// Public access modifier is required to allow Configuration class deserialization
    /// </remarks>
    public class FileStreamGetter : IStreamGetter
    {
        public Stream GetStream(string fileName)
        {
            FileStream fs = new FileStream(MapFileName(fileName), FileMode.Open, FileAccess.Read);
            return fs;
        }

        public Stream CreateStream(string fileName)
        {
            FileStream fs = new FileStream(MapFileName(fileName), FileMode.Create, FileAccess.Write);
            return fs;
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName); 
        }


        protected virtual string MapFileName(string fileName)
        {
            string path = Directory.GetCurrentDirectory();
            string fileSystemName = Path.Combine(path, @"..\..\..\..\WP7\Slovo\Slovo\", fileName);
            return fileSystemName;
        }
    }
}
