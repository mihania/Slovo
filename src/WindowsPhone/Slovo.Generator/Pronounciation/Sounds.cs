
namespace Slovo.Generator.Pronounciation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable()]
    public class Sounds
    {
        public Sounds()
        {
        }

        private List<SoundInfo> soundList = new List<SoundInfo>();

        public List<SoundInfo> SoundList
        {
            get { return this.soundList; }
        }
    }
}
