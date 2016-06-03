namespace Slovo.UI
{
    using Microsoft.HockeyApp;
    using Microsoft.Phone.Controls;
    using Slovo.Resources;
    using System;
    using System.Windows;
    using Microsoft.Phone.Shell;
    using Slovo.Core;
    using System.Collections.ObjectModel;
    using Slovo.Core.Vocabularies;

    public partial class About : PhoneApplicationPage
    {
        private Manager<PhoneStreamGetter> ManagerInstance { get { return Manager<PhoneStreamGetter>.Instance; } }

        public About()
        {
            InitializeComponent();
            this.tbCopyright.Text = String.Format(CommonResources.Copyright, Settings.AssemblyVersion.Major, Settings.AssemblyVersion.Minor);
            this.tbDescription.Text = CommonResources.VocabularyDescription;
            this.tbVocabulariesCapacity.Text = CommonResources.VocabulariesCapacity;
            this.tbLicense.Text = string.Format(CommonResources.License, Environment.NewLine);
            this.tbProgramName.Text = CommonResources.ApplicationName;
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = CommonResources.PrivateFeedback;
        }

        public static string GetPageUrl() 
        {
            return "/About.xaml";
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                new Microsoft.Phone.Tasks.MarketplaceDetailTask().Show();
            });
        }

        private void FeedbackButton_Click(object sender, EventArgs e)
        {
            ManagerInstance.TelemetryClient.TrackEvent("About.FeedbackButton_Click");
            Microsoft.HockeyApp.HockeyClient.Current.ShowFeedback();
        }
    }
}