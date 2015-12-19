namespace Slovo
{
    using Core;
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;

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
                // https://msdn.microsoft.com/en-us/library/hh763341.aspx
                var uri = new Uri("ms-appx:///" + fileName, UriKind.Absolute);
                var storageFile = StorageFile.GetFileFromApplicationUriAsync(uri);
                storageFile.AsTask<StorageFile>().Wait();

                StorageFile randomAccessStream = storageFile.GetResults();
                var task = randomAccessStream.OpenReadAsync().AsTask<IRandomAccessStreamWithContentType>();
                task.Wait();
                result = task.Result.AsStreamForRead();
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