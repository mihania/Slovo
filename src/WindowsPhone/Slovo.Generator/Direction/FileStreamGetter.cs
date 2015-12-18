
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
    public abstract class FileStreamGetter : IStreamGetter
    {

        public Stream GetStream(string fileName)
        {
            FileStream fs = new FileStream(MapFileName(fileName), FileMode.Open, FileAccess.Read);
            return fs;
        }

        public abstract string ProjectFolderName { get; }

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
            string fileSystemName = Path.Combine(path, @"..\..\..\..\WindowsPhone\" + ProjectFolderName + @"\", fileName);
            return fileSystemName;
        }
    }

    public class FileStreamGetter81 : FileStreamGetter
    {
        public override string ProjectFolderName { get { return "Slovo.UI"; } }
    }

    public class FileStreamGetter10 : FileStreamGetter
    {
        public override string ProjectFolderName { get { return "Slovo.UI_UWP"; } }
    }


}
