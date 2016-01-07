using System;

namespace Slovo.UI
{
    using Slovo.Resources;
    using System;
    using Windows.UI.Xaml;
    using Slovo.Core;
    using System.Collections.ObjectModel;
    using Slovo.Core.Vocabularies;

    public partial class About
       : Windows.UI.Xaml.Controls.Page
    {

        private Manager<PhoneStreamGetter> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter>.Instance;
            }
        }


        public About()
        {
            InitializeComponent();
            this.tbCopyright.Text = String.Format(CommonResources.Copyright, Settings.AssemblyVersion.Major, Settings.AssemblyVersion.Minor);
            this.tbDescription.Text = CommonResources.VocabularyDescription;
            this.tbVocabulariesCapacity.Text = CommonResources.VocabulariesCapacity;
            this.tbLicense.Text = string.Format(CommonResources.License, Environment.NewLine);
            this.tbProgramName.Text = CommonResources.ApplicationName;
            ((Windows.UI.Xaml.Controls.AppBarButton)((Windows.UI.Xaml.Controls.CommandBar)BottomAppBar).PrimaryCommands[0]).Label = CommonResources.PrivateFeedback;
        }

        public static string GetPageUrl()
        {
            return "/About.xaml";
        }

        private void FeedbackButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Microsoft.HockeyApp.HockeyClient.Current.ShowFeedback();
        }
    }
}