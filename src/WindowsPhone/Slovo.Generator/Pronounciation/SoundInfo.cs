namespace Slovo.Generator.Pronounciation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable()]
    public class SoundInfo
    {
        public int WordNumber { get; set; }
        public long PosInSoundFile { get; set; }
        public string Word { get; set; }

        public SoundInfo()
        {
        }

        public SoundInfo(int wordNumber, long posInSoundFile, string word)
        {
            this.WordNumber = wordNumber;
            this.PosInSoundFile = posInSoundFile;
            this.Word = word;
        }
    }
}
