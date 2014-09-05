#if EnableWp7

namespace Slovo.UI.Core.Pronounciation
{
    using System;
    using System.Net;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using Slovo.ServiceReferences;
    using System.IO;

    public class OfflinePronounciation
    {
        private const string AppId = "9608FEFA19B41804DB9C3758313EF05C5FBB9C15";
        private static LanguageServiceClient serviceClient = new LanguageServiceClient();
        internal static event EventHandler SpeakCompleted;

        static OfflinePronounciation() 
        {
            FrameworkDispatcher.Update();
            serviceClient.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(serviceClient_SpeakCompleted);
        }

        public static void SpeakAsync(string word, string languageCode) 
        {
            serviceClient.SpeakAsync(AppId, word, languageCode, "audio/wav", null);
        }

        private static void serviceClient_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            var client = new WebClient();
            client.OpenReadCompleted += ((s, args) =>
            {
                try
                {
                    SoundEffect se = SoundEffect.FromStream(args.Result);
                    se.Play();
                }
                catch (Exception)
                {
                    // Network problem
                }
            });

            try
            {
                client.OpenReadAsync(new Uri(e.Result));
            }
            catch (Exception)
            {
                // Network is not available
                Settings.IsNetworkAvailable = false;
            }

            if (SpeakCompleted != null)
            {
                SpeakCompleted(null, null);        
            }
        }
    }
}
#endif