using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Reflection;
using Slovo.Resources;
using Slovo.Core;

namespace Slovo.UI
{
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
    }
}