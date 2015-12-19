namespace Slovo.UI.PhoneCore
{
    using System.Threading.Tasks;
    using System.IO;
    using System.Xml.Serialization;
    using System;
    using Windows.Storage;

    internal class XmlIO
    {
        public static async Task<T> ReadObjectFromXmlFileAsync<T>(string filename)
        {
            // this reads XML content from a file ("filename") and returns an object  from the XML
            T objectFromXml = default(T);
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(filename);
            }
            catch (FileNotFoundException)
            {
                // file does not exist
            }

            if (file != null)
            {
                Stream stream = await file.OpenStreamForReadAsync();

#if DEBUG
                //var sr = new StreamReader(stream);
                //var content = sr.ReadToEnd();
#endif
                try
                {
                    objectFromXml = (T)serializer.Deserialize(stream);
                    stream.Dispose();
                }
                catch(Exception)
                {
                    // do nothing - bad stream.
                }
                finally
                {
                    if (stream != null)
                    {
                        try
                        {
                            stream.Dispose();
                        }
                        catch (Exception) { }
                    }
                }
            }

            return objectFromXml;
        }

        public static async Task SaveObjectToXml<T>(T objectToSave, string filename)
        {
            // https://blogs.windows.com/buildingapps/2013/06/25/file-handling-with-windows-storage-apis/

            // stores an object in XML format in file called 'filename'
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            Stream stream = await file.OpenStreamForWriteAsync();

            using (stream)
            {
                serializer.Serialize(stream, objectToSave);
            }
        }
    }
}
