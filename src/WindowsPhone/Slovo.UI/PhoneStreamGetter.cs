
using System.Windows.Interop;

namespace Slovo
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Windows;
    using Slovo.Core;


    public class PhoneStreamGetter : IStreamGetter, IDisposable
    {
        IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

        public PhoneStreamGetter()
        {

        }

        public Stream GetStream(string fileName)
        {
            Stream result = null;
            string isolatedStoragePath = GetIsolatedStoragePath(fileName);
            if (FileExists(isolatedStoragePath))
            {
                result = store.OpenFile(isolatedStoragePath, FileMode.Open);
            }
            else
            {
                result = Application.GetResourceStream(new Uri(fileName, UriKind.Relative)).Stream;
            }

            return result;
        }

        public Stream CreateStream(string path)
        {
            string isolatedStoragePath = GetIsolatedStoragePath(path);
            var directory = Path.GetDirectoryName(isolatedStoragePath);
            store.CreateDirectory(directory);
            if (store.FileExists(isolatedStoragePath))
            {
                // preventing "Operation not permitted on IsolatedStorageFileStream"
                store.DeleteFile(isolatedStoragePath);
            }

            return store.CreateFile(isolatedStoragePath);
        }

        public bool FileExists(string fileName)
        {
            return store.FileExists(fileName);
        }

        public void Dispose()
        {
            if (this.store != null)
            {
                this.store.Dispose();
            }
        }

        private string GetIsolatedStoragePath(string contentPath)
        {
            // every major.minor change is a breaking change, which requeires new file in the storage
            return string.Join("_", contentPath, Settings.AssemblyVersion.Major, Settings.AssemblyVersion.Minor);
        }
    }
}
