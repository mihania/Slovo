// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409
namespace Slovo
{
    using System;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using System.Collections.ObjectModel;


    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Application.Current.UnhandledException += Current_UnhandledException;
        }

        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Application_Launching(e);
            Frame rootFrame = Window.Current.Content as Frame;
            if (!TryToNavigateToTileReference(rootFrame, e))
            {
                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    // Set the default language
                    rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(UI.Main), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            //[WP8SL_TO_UWP] The following code was added to emulate the default behavior of
            // the back button on WP8SL
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (object sender, Windows.UI.Core.BackRequestedEventArgs backEventArgs) =>
               {
                   if (!backEventArgs.Handled && rootFrame.CanGoBack)
                   {
                       rootFrame.GoBack();
                       backEventArgs.Handled = true;
                   }
               };
            bool firstVisibilityEvent = true;
            Window.Current.CoreWindow.VisibilityChanged += (windowObj, firstVisibilityEventArgs) =>
               {
                   if (firstVisibilityEvent)
                   {
                       firstVisibilityEvent = false;
                   }
                   else
                   {
                       if (firstVisibilityEventArgs.Visible)
                       {
                           Application_Activated(null, null);
                        // Refresh the current page
                        Frame currentFrame = Window.Current.Content as Frame;
                           if (currentFrame != null)
                           {
                               var navstate = currentFrame.GetNavigationState();
                               currentFrame.SetNavigationState(navstate);
                           }
                       }
                       else
                       {
                           Application_Deactivated(null, null);
                       }
                   }
               };
        }

        bool TryToNavigateToTileReference(Frame rootFrame, LaunchActivatedEventArgs e)
        {
            switch (e.TileId)
            {
                case "_About.xaml":
                    rootFrame.Navigate(typeof(Slovo.UI.About), e.Arguments);
                    return true;
                case "_DirectionArticle.xaml":
                    rootFrame.Navigate(typeof(Slovo.DirectionArticle), e.Arguments);
                    return true;
                case "_Main.xaml":
                    rootFrame.Navigate(typeof(Slovo.UI.Main), e.Arguments);
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
            Application_Deactivated(sender, e);
            Application_Closing(sender, e);
        }

        private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance;
            }
        }

        private void OnClosing()
        {
            if (ManagerInstance.History.IsChanged)
            {
                ManagerInstance.History.Save();
            }
        }

        void Application_Launching(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
        {
            //WINDOWS_PHONE_SL_TO_UWP: (1101) Microsoft.Phone.Shell.LaunchingEventArgs was not upgraded
            Settings.Init();
            // Call this on launch to initialise the feedback helper
            NokiaFeedbackDemo.Helpers.FeedbackHelper.Default.Launching();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, System.Object e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private async void Application_Deactivated(object sender, System.Object e)
        {
            this.OnClosing();
        }

        async void Application_Closing(object obj, Windows.ApplicationModel.SuspendingEventArgs args)
        {
            this.OnClosing();
        }
    }
}