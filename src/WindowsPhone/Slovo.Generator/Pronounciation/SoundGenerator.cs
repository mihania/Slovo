namespace Slovo.Generator.Pronounciation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework;
    using ServiceReferences;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;


    public class SoundGenerator
    {
        private const string AppId = "9608FEFA19B41804DB9C3758313EF05C5FBB9C15";

        static SoundGenerator()
        {
        }

        public static void GenerateSound(List<Article> articles, int vocName, String outputFileName, string outputXmlFileName)
        {
            FrameworkDispatcher.Update();
            LanguageServiceClient serviceClient = new LanguageServiceClient();
            Sounds sounds = new Sounds();
            using (FileStream outputStream = new FileStream(outputFileName, FileMode.Create))
            {
                using (BinaryWriter listBinaryWriter = new BinaryWriter(outputStream))
                {
                    int i = 0;
                    foreach (Article a in articles)
                    {
                        // ToDo: Change null to article sense
                        // var sense = a.Sense;
                        var sense = "ToDo: Put sense here!!";
                        string url = serviceClient.Speak(AppId, sense, GetLanguageCode(vocName), "audio/wav", null);
                        WebClient webClient = new WebClient();

                        byte[] array = webClient.DownloadData(url);

                        SoundInfo soundInfo = new SoundInfo(i, outputStream.Position, sense);
                        sounds.SoundList.Add(soundInfo);
                        i++;
                        if (i % 100 == 0)
                        {
                            Console.WriteLine("Generating sound for the word: {0}", sense);
                        }

                        listBinaryWriter.Write(array.Length);
                        listBinaryWriter.Write(array, 0, array.Length);
                    }
                }
            }


            XmlSerializer serializer = new XmlSerializer(typeof(Sounds));
            using (Stream writer = new FileStream(outputXmlFileName, FileMode.Create))
            {
                serializer.Serialize(writer, sounds);
            }
        }

       

        private static string GetLanguageCode(int vocName)
        {
            string result = null;
            switch (vocName)
            {
                case (int)VocabularyId.EngRus:
                    result = "en";
                    break;
                case (int)VocabularyId.RusEng:
                    result = "ru";
                    break;
                default:
                    throw new ArgumentException("vocName");
            }

            return result;
        }

        internal static void Save1File(long pos, string inputFileName, string outputFileName)
        {
            using (Stream stream = new FileStream(inputFileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    stream.Seek(pos, SeekOrigin.Begin);
                    int length = br.ReadInt32();
                    byte[] array = new byte[length];
                    br.Read(array, 0, length);
                    using (FileStream output = new FileStream(outputFileName, FileMode.Create))
                    {
                        output.Write(array, 0, length);
                    }
                }
            }
        }
    }
}
