namespace Slovo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using Slovo.Core;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using Slovo.UI;
    using Slovo.Core.Config;


    public partial class SettingsControl : UserControl
    {
        internal const string ControlName = "VocabulariesPivotItem";

        public SettingsControl(Main mainPage)
        {
            this.InitializeComponent();
            this.MainPage = mainPage;
            this.OnLoading();
            mainPage.ApplicationBarClick += mainPage_ApplicationBarClick;
        }

        void mainPage_ApplicationBarClick(object sender, EventArgs e)
        {
            var button = (ApplicationBarButton)sender;
            if (button == ApplicationBarButton.Apply)
            {
                this.ApplyButton_Click(button, null);
            }
        }

        internal Manager<PhoneStreamGetter> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter>.Instance;
            }
        }

        internal Slovo.UI.Main MainPage { get; private set; }

        internal void OnLoading()
        {
            this.DirectionList.ItemsSource = ManagerInstance.Configuration.Directions.Clone();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => this.PerformanceProgressBar.IsIndeterminate = true);
            var newDirections = (DirectionList<PhoneStreamGetter>)this.DirectionList.ItemsSource;
            var equalStatus = ManagerInstance.UpdateDirections(newDirections);

            switch (equalStatus)
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

            Dispatcher.BeginInvoke(() => this.PerformanceProgressBar.IsIndeterminate = false);
        }
    }
}
