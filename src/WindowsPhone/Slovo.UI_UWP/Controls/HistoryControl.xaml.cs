namespace Slovo
{
    using Slovo.Core;
    using Slovo.Core.Vocabularies;
    using System.Collections.ObjectModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using System.Threading.Tasks;

    public partial class HistoryControl : UserControl
    {
        public HistoryControl()
        {
            InitializeComponent();
        }

        private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance;
            }
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            list.Items.Clear();
            await ManagerInstance.History.EnsurePersistenceHistoryLoaded();
            foreach (var item in ManagerInstance.History.Items)
            {
                list.Items.Add(item);
            }
        }

        private void list_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                var directionArticle = (Slovo.Core.Directions.DirectionArticle)list.SelectedItem;
                ManagerInstance.History.CurrentIndex = ManagerInstance.History.Items.IndexOf(directionArticle);

                var parameters = new DirectionArticleNavigateParams()
                {
                    DirectionId = directionArticle.DirectionId,
                    Sense = directionArticle.Sense,
                    DirectionOffsets = directionArticle.DefinitionOffsets
                };

                (Window.Current.Content as Frame).Navigate(typeof(DirectionArticle2), parameters);
            }
        }

        private void MenuItemRemoveClick(object sender, RoutedEventArgs e)
        {
            var directionArticle = (Slovo.Core.Directions.DirectionArticle)(sender as Windows.UI.Xaml.Controls.MenuFlyoutItem).DataContext;
            if (directionArticle != null)
            {
                int index = ManagerInstance.History.Items.IndexOf(directionArticle);
                ManagerInstance.History.Remove(directionArticle);
                list.Items.RemoveAt(index);
            }
        }
    }
}