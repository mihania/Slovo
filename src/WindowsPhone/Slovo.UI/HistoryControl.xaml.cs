namespace Slovo
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Slovo.Core;
    using System.Collections.ObjectModel;
    using Slovo.Core.Vocabularies;


    public partial class HistoryControl : UserControl
    {
        public HistoryControl()
        {
            InitializeComponent();
        }

        private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance { get { return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance; } }


        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            list.Items.Clear();
            foreach (var item in ManagerInstance.History.Items)
            {
                list.Items.Add(item);
            }
        }

        private void list_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                var directionArticle = (Slovo.Core.Directions.DirectionArticle)list.SelectedItem;
                ManagerInstance.History.CurrentIndex = ManagerInstance.History.Items.IndexOf(directionArticle);
                Uri uri = new Uri(Slovo.DirectionArticle.GetPageUrl(directionArticle.DirectionId, directionArticle.Sense, directionArticle.DefinitionOffsets), UriKind.Relative);
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(uri);
            }
        }

        private void MenuItemRemoveClick(object sender, RoutedEventArgs e)
        {
            var directionArticle = (Slovo.Core.Directions.DirectionArticle)(sender as MenuItem).DataContext;
            if (directionArticle != null)
            {
                int index = ManagerInstance.History.Items.IndexOf(directionArticle);
                ManagerInstance.History.Remove(directionArticle);
                list.Items.RemoveAt(index);
            }
        }
    }
}
