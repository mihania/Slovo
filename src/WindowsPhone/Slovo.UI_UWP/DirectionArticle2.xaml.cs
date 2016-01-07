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
        private DirectionArticleNavigateParams navigateParams;
        private DirectionArticle currentDirectionArticle;

        public DirectionArticle2()
        {
            this.InitializeComponent();
            BackButton.Label = CommonResources.Back;
            SpeakButton.Label = CommonResources.Listen;
            NextButton.Label = CommonResources.Next;
            SearhButton.Label = CommonResources.Search;
            PivotArticle.PivotItemLoading += PivotArticle_PivotItemLoading;
            this.Loaded += DirectionArticle_Loaded;
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

        private Manager<PhoneStreamGetter> ManagerInstance
        {
            get
            {
                return Manager<PhoneStreamGetter>.Instance;
            }
        }

        private Direction<PhoneStreamGetter> Direction
        {
            get
            {
                return ManagerInstance.GetDirection(this.currentDirectionArticle.DirectionId, LoadingState.Loaded);
            }
        }

        private void ShowApplicationBar(bool historyWatch)
        {
            BackButton.Visibility = historyWatch ? Visibility.Visible : Visibility.Collapsed;
            NextButton.Visibility = historyWatch ? Visibility.Visible : Visibility.Collapsed;
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
                    // DirectionArticle is retrived from History where Definition is not stored. We need to retrieve article again and update its definition
                    coreArticle.Definition = vocabulary.GetArticle(coreArticle.Offset).Definition;
                }

                article.Definition = @"{\rtf1 " +coreArticle.Definition + "}";
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

        private void SearchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var pivotItem = ((PivotItem)PivotArticle.SelectedItem);
            if (pivotItem != null)
            {
                var article = (UI.Controls.Article)pivotItem.Content;
                if (!string.IsNullOrEmpty(article.SelectedText))
                {
                    var selectedText = article.SelectedText.Trim();

                }
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