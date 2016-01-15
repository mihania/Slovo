namespace Slovo
{
    using Slovo.Core;
    using Slovo.Core.Config;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.UI;
    using System;
    using System.Collections.ObjectModel;

    public partial class SettingsControl : Windows.UI.Xaml.Controls.UserControl
   {
      internal const string ControlName = "VocabulariesPivotItem";

      public SettingsControl(Main mainPage)
      {
         this.InitializeComponent();
         this.MainPage = mainPage;
         this.OnLoading();
      }

      void mainPage_ApplicationBarClick(object sender, EventArgs e)
      {
         //var button = (ApplicationBarButton)sender;
         //if ( button == ApplicationBarButton.Apply )
         //{
         //   this.ApplyButton_Click(button, null);
         //}
      }

      internal Manager<PhoneStreamGetter> ManagerInstance
      {
         get
         {
            return Manager<PhoneStreamGetter>.Instance;
         }
      }

      internal Main MainPage { get; private set; }

      internal void OnLoading()
      {
         this.DirectionList.ItemsSource = ManagerInstance.Configuration.Directions.Clone();
      }

      private async void ApplyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
      {
         await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.PerformanceProgressBar.IsIndeterminate = true);
         var newDirections = (DirectionList<PhoneStreamGetter>)this.DirectionList.ItemsSource;
         var equalStatus = ManagerInstance.UpdateDirections(newDirections);
         switch ( equalStatus )
         {
            case EqualStatus.NotEqual:
            case EqualStatus.SameEnabledVocabularies:
               this.OnLoading();
               this.MainPage.InitDirectionSync();
               ManagerInstance.Configuration.Save();
               break;
            default:
               break;
         }
         await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.PerformanceProgressBar.IsIndeterminate = false);
      }
   }
}