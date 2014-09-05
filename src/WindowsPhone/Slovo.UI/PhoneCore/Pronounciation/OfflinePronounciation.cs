#if EnableWp7
#else
namespace Slovo.UI.Core.Pronounciation
{
    using System;
    using Windows.Phone.Speech.Synthesis;

    internal class OfflinePronounciation
    {
        public static async void SpeakAsync(string word, string languageCode)
        {
            SpeechSynthesizer speaker = new SpeechSynthesizer();

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

            if (currentVoice != null)
            {
                speaker.SetVoice(currentVoice);
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
    }
}
#endif