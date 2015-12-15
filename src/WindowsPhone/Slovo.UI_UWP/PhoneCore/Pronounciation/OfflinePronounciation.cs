using WindowsPhoneUWP.UpgradeHelpers;
using Slovo.Core;

namespace Slovo.UI.Core.Pronounciation
{
   using System;
   using System.Threading.Tasks;

   internal class OfflinePronounciation
   {

      private static async Task SpeakAsync(Windows.Media.SpeechSynthesis.SpeechSynthesizer speaker, string word, Windows.Media.SpeechSynthesis.VoiceInformation voice)
      {
         if ( voice != null )
         {
            speaker.Voice = voice;
            try
            {
               await speaker.SpeakTextHelperAsync(word);
            }
            catch ( Exception )
            {
            // there are multiple failures here because of unknown reason.
            // swallowing for now to minimize crash count rate
            }
         }
      }

      public static async void SpeakAsync(string word, string languageCode)
      {
         using ( Windows.Media.SpeechSynthesis.SpeechSynthesizer speaker = new Windows.Media.SpeechSynthesis.SpeechSynthesizer() )
         {
            Windows.Media.SpeechSynthesis.VoiceInformation currentVoice = GetByLanguageCode(speaker, languageCode);
            await SpeakAsync(speaker, word, currentVoice);
         }
      }

      public static bool SupportPronounciation(string languageCode)
      {
         using ( Windows.Media.SpeechSynthesis.SpeechSynthesizer speaker = new Windows.Media.SpeechSynthesis.SpeechSynthesizer() )
         {
            return GetByLanguageCode(speaker, languageCode) != null;
         }
      }

      private static Windows.Media.SpeechSynthesis.VoiceInformation GetByLanguageCode(Windows.Media.SpeechSynthesis.SpeechSynthesizer speaker, string languageCode)
      {
         var allVoices = Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices;
         Windows.Media.SpeechSynthesis.VoiceInformation currentVoice = null;
         for ( int i = 0; i < allVoices.Count; i++ )
         {
            if ( Common.StringCompare(allVoices[i].Language, languageCode) == 0 )
            {
               currentVoice = allVoices[i];
               break;
            }
         }
         return currentVoice;
      }

   }

}