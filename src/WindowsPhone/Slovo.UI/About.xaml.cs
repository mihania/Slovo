namespace Slovo.UI
{
    using HockeyApp;
    using Microsoft.Phone.Controls;
    using Slovo.Resources;
    using System;
    using System.Windows;

    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            this.tbCopyright.Text = String.Format(CommonResources.Copyright, Settings.AssemblyVersion.Major, Settings.AssemblyVersion.Minor);
            this.tbDescription.Text = CommonResources.VocabularyDescription;
            this.tbVocabulariesCapacity.Text = CommonResources.VocabulariesCapacity;
            this.tbLicense.Text = string.Format(CommonResources.License, Environment.NewLine);
            this.tbProgramName.Text = CommonResources.ApplicationName;
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
            HockeyClient.Current.ShowFeedback();
        }
    }
}