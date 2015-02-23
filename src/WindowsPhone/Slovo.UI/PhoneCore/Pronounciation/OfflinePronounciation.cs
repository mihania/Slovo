#if EnableWp7
#else
namespace Slovo.UI.Core.Pronounciation
{
    using System;
    using Windows.Phone.Speech.Synthesis;
    using System.Threading.Tasks;

    internal class OfflinePronounciation
    {
        private static async Task SpeakAsync(SpeechSynthesizer speaker, string word, VoiceInformation voice)
        {
            if (voice != null)
            {
                speaker.SetVoice(voice);
                try
                {
                    await speaker.SpeakTextAsync(word);
                }
                catch (Exception)
                {
                    // there are multiple failures here because of unknown reason.
                    // swallowing for now to minimize crash count rate
                }
            }
        }

        public static async void SpeakAsync(string word, string languageCode)
        {
            using (SpeechSynthesizer speaker = new SpeechSynthesizer())
            {
                VoiceInformation currentVoice = GetByLanguageCode(speaker, languageCode);
                await SpeakAsync(speaker, word, currentVoice);
            }
        }

        public static bool SupportPronounciation(string languageCode)
        {
            using (SpeechSynthesizer speaker = new SpeechSynthesizer())
            {
                return GetByLanguageCode(speaker, languageCode) != null;
            }
        }

        private static VoiceInformation GetByLanguageCode(SpeechSynthesizer speaker, string languageCode)
        {
            var allVoices = InstalledVoices.All;
            VoiceInformation currentVoice = null;
            for (int i = 0; i < allVoices.Count; i++)
            {
                if (string.Compare(allVoices[i].Language, languageCode, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    currentVoice = allVoices[i];
                    break;
                }
            }

            return currentVoice;
        }
    }
}
#endif