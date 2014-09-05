namespace Slovo.UI
{
    using Microsoft.Phone.Controls;
    using System;
    using System.Windows;
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;

    public partial class Buy : PhoneApplicationPage
    {
        public Buy()
        {
            InitializeComponent();
        }
        public static string GetPageUrl(string word) 
        {
            return String.Format("/Buy.xaml?selectedItem={0}", word);
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                new Microsoft.Phone.Tasks.MarketplaceDetailTask().Show();
            });
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = this.NavigationContext.QueryString["selectedItem"];
            this.tbDescription.Text = CommonResources.BuyProgramDescription;
            this.btnBuy.Content = CommonResources.Buy;
        }
    }
}