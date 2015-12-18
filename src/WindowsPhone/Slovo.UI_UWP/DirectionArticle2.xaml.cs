namespace Slovo
{
    using Slovo.Core;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using Slovo.UI.Core.Pronounciation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    public partial class DirectionArticle2 : Page
    {
        private const int BackButtonIndex = 0;
        private const int ListenButtonIndex = 1;
        private const int NextButtonIndex = 2;
        private DirectionArticleNavigateParams navigateParams;
        private DirectionArticle currentDirectionArticle;

        public DirectionArticle2()
        {
            this.InitializeComponent();
            ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[BackButtonIndex]).Label = CommonResources.Back;
            ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[ListenButtonIndex]).Label = CommonResources.Listen;
            ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[NextButtonIndex]).Label = CommonResources.Next;
            PivotArticle.PivotItemLoading += PivotArticle_PivotItemLoading;
            this.Loaded += new Windows.UI.Xaml.RoutedEventHandler(DirectionArticle_Loaded);
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">An object that contains the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigateParams = (DirectionArticleNavigateParams)e.Parameter;
            if (this.navigateParams == null)
            {
                throw new NullReferenceException("navigateParams is null");
            }

            base.OnNavigatedTo(e);
        }

        private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance;
            }
        }

        private Direction<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> Direction
        {
            get
            {
                return ManagerInstance.GetDirection(this.currentDirectionArticle.DirectionId, LoadingState.Loaded);
            }
        }

        private void ShowApplicationBar(bool historyWatch)
        {
            ((CommandBar)BottomAppBar).Visibility = true ? Visibility.Visible : Visibility.Collapsed;
            int listenButtonIndex = 0;
            if (!historyWatch)
            {
                if (((CommandBar)BottomAppBar).PrimaryCommands.Count > 1)
                {
                    ((CommandBar)BottomAppBar).PrimaryCommands.RemoveAt(0);
                    ((CommandBar)BottomAppBar).PrimaryCommands.RemoveAt(1);
                }
            }
            else
            {
                listenButtonIndex = ListenButtonIndex;
            }
            if (((CommandBar)BottomAppBar).PrimaryCommands.Count == 1 && !Direction.SupportPronounciation)
            {
                ((CommandBar)BottomAppBar).PrimaryCommands.RemoveAt(0);
                ((CommandBar)BottomAppBar).Visibility = false ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                ((CommandBar)BottomAppBar).Visibility = true ? Visibility.Visible : Visibility.Collapsed;
                ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[listenButtonIndex]).IsEnabled = Direction.SupportPronounciation;
            }
        }

        private void PivotArticle_PivotItemLoading(Pivot sender, PivotItemEventArgs e)
        {
            if (e.Item.DataContext != null)
            {
                UI.Controls.Article article = new UI.Controls.Article();
                var keyValuePair = (KeyValuePair<int, int>)e.Item.DataContext;
                var vocabulary = ManagerInstance.GetVocabulary(keyValuePair.Key);
                int offset = keyValuePair.Value;
                var coreArticle = vocabulary.GetArticle(offset);
                if (coreArticle.Definition == null)
                {
                    // DirectionArticle is retrived from History where Definition is not stored. We need to retrieve article again 
                    // and update its definition
                    coreArticle.Definition = vocabulary.GetArticle(coreArticle.Offset).Definition;
                }

                coreArticle.Definition = coreArticle.Definition.Replace("\n", @"\line ");

                article.Definition = @"{\rtf1 
                {\colortbl;
                \red255\green0\blue0;
                }" + coreArticle.Definition + "}";

                e.Item.Content = article;
                e.Item.DataContext = null; // mark article as loaded
            }
        }

        /// <summary>
        /// Loads article
        /// </summary>
        /// <param name="historyWatch">True if article is loaded in history window, otherwise false</param>
        private void LoadDirectionArticle(DirectionArticle directionArticle, bool historyWatch)
        {
            this.currentDirectionArticle = directionArticle;
            PivotArticle.Title = this.currentDirectionArticle.Sense;
            PivotArticle.Items.Clear();
            this.ShowApplicationBar(historyWatch);
            var direction = ManagerInstance.GetDirection(directionArticle.DirectionId, LoadingState.Loaded);
            foreach (var vocabulary in direction.Vocabularies)
            {
                if (vocabulary.IsEnabled || historyWatch)
                {
                    int offset;
                    if (directionArticle.DefinitionOffsets.TryGetValue(vocabulary.VocabularyId, out offset))
                    {
                        var keyValuePair = new KeyValuePair<int, int>(vocabulary.VocabularyId, offset);
                        
                        // setting name property because of system.argumentexception in MS.Internal.XcpImports.CheckHResult
                        // based on suggestion in http://stackoverflow.com/questions/7413293/unhandledexception-in-wp7
                        PivotItem pivot = new PivotItem()
                        {
                            DataContext = keyValuePair,
                            Header = vocabulary.PivotHeader, 
                            Name = Guid.NewGuid().ToString()
                        };
                        pivot.DataContext = keyValuePair;
                        PivotArticle.Items.Add(pivot);
                    }
                }
            }
        }

        private void DirectionArticle_Loaded(object sender, RoutedEventArgs e)
        {
            bool historyWatch = false;
            DirectionArticle directionArticle;
            if (this.navigateParams.DirectionArticleId.HasValue)
            {
                // opened from search window
                directionArticle = ManagerInstance.GetDirection(this.navigateParams.DirectionId, LoadingState.Loaded).GetArticle(this.navigateParams.DirectionArticleId.Value);
                ManagerInstance.History.Add(directionArticle);
            }
            else
            {
                // opened in history window
                directionArticle = new DirectionArticle(navigateParams.Sense)
                {
                    DefinitionOffsets = navigateParams.DirectionOffsets,
                    DirectionId = navigateParams.DirectionId
                };
                
                historyWatch = true;
            }

            this.LoadDirectionArticle(directionArticle, historyWatch);
        }

        private void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            OfflinePronounciation.SpeakAsync(currentDirectionArticle.Sense, this.Direction.SourceLanguageCode);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ManagerInstance.History.HasPrevious)
            {
                this.LoadDirectionArticle(this.ManagerInstance.History.Previous, true);
            }
        }

        private void NextButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.ManagerInstance.History.HasNext)
            {
                this.LoadDirectionArticle(this.ManagerInstance.History.Next, true);
            }
        }
    }

    public class DirectionArticleNavigateParams
    {
        public Dictionary<int, int> DirectionOffsets { get; set; }

        public int DirectionId { get; set; }

        public string Sense { get; set; }

        public int? DirectionArticleId { get; set; }
    }
}