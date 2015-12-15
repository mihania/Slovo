namespace NokiaFeedbackDemo.Controls
{
    using NokiaFeedbackDemo.Helpers;
    using Slovo;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Windows.UI.Xaml;

    public partial class FeedbackOverlay : Windows.UI.Xaml.Controls.UserControl
   {
// Use this from XAML to control whether animation is on or off
#region EnableAnimation Dependency Property
      public static readonly Windows.UI.Xaml.DependencyProperty EnableAnimationProperty = Windows.UI.Xaml.DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(true, null));

      public static void SetEnableAnimation(FeedbackOverlay element, bool value)
      {
         element.SetValue(EnableAnimationProperty, value);
      }

      public static bool GetEnableAnimation(FeedbackOverlay element)
      {
         return (bool)element.GetValue(EnableAnimationProperty);
      }
#endregion

// Use this for MVVM binding IsVisible
#region IsVisible Dependency Property
      public static readonly Windows.UI.Xaml.DependencyProperty IsVisibleProperty = Windows.UI.Xaml.DependencyProperty.Register("IsVisible", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(false, null));

      public static void SetIsVisible(FeedbackOverlay element, bool value)
      {
         element.SetValue(IsVisibleProperty, value);
      }

      public static bool GetIsVisible(FeedbackOverlay element)
      {
         return (bool)element.GetValue(IsVisibleProperty);
      }
#endregion

// Use this for MVVM binding IsNotVisible
#region IsNotVisible Dependency Property
      public static readonly Windows.UI.Xaml.DependencyProperty IsNotVisibleProperty = Windows.UI.Xaml.DependencyProperty.Register("IsNotVisible", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(true, null));

      public static void SetIsNotVisible(FeedbackOverlay element, bool value)
      {
         element.SetValue(IsNotVisibleProperty, value);
      }

      public static bool GetIsNotVisible(FeedbackOverlay element)
      {
         return (bool)element.GetValue(IsNotVisibleProperty);
      }
#endregion

      // Use this for detecting visibility change on code
      public event EventHandler VisibilityChanged = null;
      private Windows.UI.Xaml.Controls.Frame _rootFrame = null;

      private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance
      {
         get
         {
            return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance;
         }
      }

      public string Title
      {
         set
         {
            if ( this.title.Text != value )
            {
               this.title.Text = value;
            }
         }
      }

      public string Message
      {
         set
         {
            if ( this.message.Text != value )
            {
               this.message.Text = value;
            }
         }
      }

      public string NoText
      {
         set
         {
            if ( (string)this.noButton.Content != value )
            {
               this.noButton.Content = value;
            }
         }
      }

      public string YesText
      {
         set
         {
            if ( (string)this.yesButton.Content != value )
            {
               this.yesButton.Content = value;
            }
         }
      }


      public FeedbackOverlay()
      {
         InitializeComponent();
         this.yesButton.Click += yesButton_Click;
         this.noButton.Click += noButton_Click;
         this.Loaded += FeedbackOverlay_Loaded;

         //ToDo: Verify migration!
         // hideContent.Completed += hideContent_Completed;
      }

      private void FeedbackOverlay_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
      {
         this.AttachBackKeyPressed();
         if ( FeedbackOverlay.GetEnableAnimation(this) )
         {
            this.LayoutRoot.Opacity = 0;
            this.xProjection.RotationX = 90;
         }
         if ( FeedbackHelper.Default.State == FeedbackState.FirstReview )
         {
            this.SetVisibility(true);
            this.SetupFirstMessage();
            if ( FeedbackOverlay.GetEnableAnimation(this) )
               this.showContent.Begin();
         }
         else if ( FeedbackHelper.Default.State == FeedbackState.SecondReview )
         {
            this.SetVisibility(true);
            this.SetupSecondMessage();
            if ( FeedbackOverlay.GetEnableAnimation(this) )
               this.showContent.Begin();
         }
         else
         {
            this.SetVisibility(false);
         }
      }

      private void AttachBackKeyPressed()
      {
         // Detect back pressed
         if ( this._rootFrame == null )
         {
            this._rootFrame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            
            // ToDo: UWP Migration uncomment
            // this._rootFrame.BackKeyPress += FeedbackOverlay_BackKeyPress;
         }
      }

      private void FeedbackOverlay_BackKeyPress(object sender, CancelEventArgs e)
      {
         // If back is pressed whilst open, close and cancel back to stop app exiting
         if ( this.Visibility == Windows.UI.Xaml.Visibility.Visible )
         {
            this.OnNoClick();
            e.Cancel = true;
         }
      }

      private void SetupFirstMessage()
      {
         this.Title = AppResources.RatingTitle;
         this.Message = AppResources.RatingMessage1;
         this.YesText = AppResources.RatingYes;
         this.NoText = AppResources.RatingNo;
      }

      private void SetupSecondMessage()
      {
         this.Title = AppResources.RatingTitle;
         this.Message = AppResources.RatingMessage2;
         this.YesText = AppResources.RatingYes;
         this.NoText = AppResources.RatingNo;
      }

      private void SetupFeedbackMessage()
      {
         this.Title = AppResources.FeedbackTitle;
         this.Message = AppResources.FeedbackMessage1;
         this.YesText = AppResources.FeedbackYes;
         this.NoText = AppResources.FeedbackNo;
      }

      private void noButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
      {
         this.OnNoClick();
      }

      private void OnNoClick()
      {
         if ( FeedbackOverlay.GetEnableAnimation(this) )
            this.hideContent.Begin();
         else
            this.ShowFeedback();
      }

      private void hideContent_Completed(object sender)
      {
         this.ShowFeedback();
      }

      private void ShowFeedback()
      {
         if ( FeedbackHelper.Default.State == FeedbackState.FirstReview )
         {
            this.SetupFeedbackMessage();
            FeedbackHelper.Default.State = FeedbackState.Feedback;
            if ( FeedbackOverlay.GetEnableAnimation(this) )
               this.showContent.Begin();
         }
         else
         {
            this.SetVisibility(false);
         }
      }

      private async void yesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
      {
         this.SetVisibility(false);
         if ( FeedbackHelper.Default.State == FeedbackState.FirstReview )
         {
            await this.Review();
         }
         else if ( FeedbackHelper.Default.State == FeedbackState.SecondReview )
         {
            await this.Review();
         }
         else if ( FeedbackHelper.Default.State == FeedbackState.Feedback )
         {
            await this.Feedback();
         }
      }

      private async System.Threading.Tasks.Task Review()
      {
         FeedbackHelper.Default.Reviewed();
         await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.Name));
      }

      private async System.Threading.Tasks.Task Feedback()
      {
         // Body text including hardware, firmware and software info
         string body = string.Format(AppResources.FeedbackBody, ((new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation()).SystemProductName), ((new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation()).SystemManufacturer), ((new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation()).SystemFirmwareVersion), ((new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation()).SystemHardwareVersion), Settings.AssemblyVersion);
         // Email task
         var email = new Windows.ApplicationModel.Email.EmailMessage()
            {
               To =
               {
                  new Windows.ApplicationModel.Email.EmailRecipient()
               }
            };
         email.To[0].Address = AppResources.FeedbackTo;
         email.Subject = AppResources.FeedbackSubject;
         email.Body = body;
         await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(email);
      }

      private void SetVisibility(bool visible)
      {
         if ( visible )
         {
            this.LayoutRoot.Opacity = 0;
            FeedbackOverlay.SetIsVisible(this, true);
            FeedbackOverlay.SetIsNotVisible(this, false);
            this.Visibility = Windows.UI.Xaml.Visibility.Visible;
         }
         else
         {
            this.LayoutRoot.Opacity = 0;
            FeedbackOverlay.SetIsVisible(this, false);
            FeedbackOverlay.SetIsNotVisible(this, true);
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
         }
         this.OnVisibilityChanged();
      }

      private void OnVisibilityChanged()
      {
         if ( this.VisibilityChanged != null )
            this.VisibilityChanged(this, new EventArgs());
      }

   }

}