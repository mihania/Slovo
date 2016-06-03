namespace Slovo
{
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Slovo.Core;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using Slovo.UI.Core.Pronounciation;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;

    public partial class DirectionArticle : PhoneApplicationPage
    {
        private const int BackButtonIndex = 0;
        private const int ListenButtonIndex = 1;
        private const int NextButtonIndex = 2;

        public DirectionArticle()
        {
            this.InitializeComponent();

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[BackButtonIndex]).Text = CommonResources.Back;
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[ListenButtonIndex]).Text = CommonResources.Listen;
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[NextButtonIndex]).Text = CommonResources.Next;

            PivotArticle.LoadingPivotItem += new EventHandler<PivotItemEventArgs>(PivotArticle_LoadingPivotItem);
            this.Loaded += new RoutedEventHandler(DirectionArticle_Loaded);
        }

        private Manager<PhoneStreamGetter> ManagerInstance { get { return Manager<PhoneStreamGetter>.Instance; } }

        private Direction<PhoneStreamGetter> Direction
        {
            get
            {
                int directionId = (int)int.Parse(this.NavigationContext.QueryString["directionId"]);
                return ManagerInstance.GetDirection(directionId, LoadingState.Loaded);
            }
        }

        private void ShowApplicationBar(bool historyWatch)
        {
            this.ApplicationBar.IsVisible = true;
            int listenButtonIndex = 0;
            if (!historyWatch)
            {
                if (ApplicationBar.Buttons.Count > 1)
                {
                    ApplicationBar.Buttons.RemoveAt(0);
                    ApplicationBar.Buttons.RemoveAt(1);
                }
            }
            else
            {
                listenButtonIndex = ListenButtonIndex;
            }

            if (this.ApplicationBar.Buttons.Count == 1 && !Direction.SupportPronounciation)
            {
                // no need to show the speaker button as it is the only one on application bar and speaker does not work.
                ApplicationBar.Buttons.RemoveAt(0);
                ApplicationBar.IsVisible = false;
            }
            else
            {
                ApplicationBar.IsVisible = true;
                ((ApplicationBarIconButton)this.ApplicationBar.Buttons[listenButtonIndex]).IsEnabled = Direction.SupportPronounciation;
            }
            
        }

        public static string GetPageUrl(int directionId, int directionArticleId)
        {
            return String.Format("/DirectionArticle.xaml?directionId={0}&directionArticleId={1}", (int)directionId, directionArticleId);
        }

        public static string GetPageUrl(int directionId, string sense, Dictionary<int, int> definitionOffsets)
        {
            var result = new StringBuilder(String.Format("/DirectionArticle.xaml?directionId={0}&sense={1}&do=", (int)directionId, sense));

            int current = 0;
            foreach (var pair in definitionOffsets)
            {
                result.Append((int)pair.Key).Append("|").Append(pair.Value);
                current++;
                if (current < definitionOffsets.Count)
                {
                    result.Append("|");
                }
            }

            return result.ToString();
        }

        private Dictionary<int, int> ParseDefinitionOffsets(string definitionOffsets)
        {
            var result = new Dictionary<int, int>();
            string[] array = definitionOffsets.Split('|');
            for (int i = 0; i < array.Length; i = i + 2)
            {
                result[(int)int.Parse(array[i])] = int.Parse(array[i + 1]);
            }

            return result;
        }

        private void PivotArticle_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item.DataContext != null)
            {   
                UI.Article article = new UI.Article();
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

                article.Definition = coreArticle.Definition;
                e.Item.Content = article;
                e.Item.DataContext = null; // mark article as loaded
            }
        }

        /// <summary>
        /// Loads article
        /// </summary>
        /// <param name="historyWatch">True if article is loaded in history window, otherwise false</param>
        private void LoadDirectionArticle(Direction<PhoneStreamGetter> direction, string sense, Dictionary<int, int> offsets, bool historyWatch)
        {
            tbWord.Text = sense;
            PivotArticle.Items.Clear();
            this.NavigationContext.QueryString["directionId"] = ((int)direction.Id).ToString();
            this.ShowApplicationBar(historyWatch);

            
            foreach (var vocabulary in direction.Vocabularies)
            {
                if (vocabulary.IsEnabled || historyWatch)
                {
                    int offset;
                    if (offsets.TryGetValue(vocabulary.VocabularyId, out offset))
                    {
                        var keyValuePair = new KeyValuePair<int, int>(vocabulary.VocabularyId, offset);
                        
                        // setting name property because of system.argumentexception in MS.Internal.XcpImports.CheckHResult
                        // based on suggestion in http://stackoverflow.com/questions/7413293/unhandledexception-in-wp7
                        PivotItem pivot = new PivotItem() { DataContext = keyValuePair, Header = vocabulary.PivotHeader, Name = Guid.NewGuid().ToString()};
                        pivot.DataContext = keyValuePair;
                        PivotArticle.Items.Add(pivot);
                    }
                }
            }
        }

        private void DirectionArticle_Loaded(object sender, RoutedEventArgs e)
        {
            var direction = this.Direction;
            
            string directionArticleIdString;
            string sense = string.Empty;
            Dictionary<int, int> definitionOffsets = null;
            bool historyWatch = false;

            
            if (this.NavigationContext.QueryString.TryGetValue("directionArticleId", out directionArticleIdString))
            {
                int directionArticleId = int.Parse(directionArticleIdString);
                var directionArticle = direction.GetArticle(directionArticleId);
                sense = directionArticle.Sense;
                definitionOffsets = directionArticle.DefinitionOffsets;
                ManagerInstance.History.Add(directionArticle);
            }
            else
            {
                
                if (!this.NavigationContext.QueryString.TryGetValue("sense", out sense))
                {
                    throw new ArgumentNullException("sense");
                }

                string definitionOffsetsString;
                if (!this.NavigationContext.QueryString.TryGetValue("do", out definitionOffsetsString)) 
                {
                    throw new ArgumentNullException("do");
                }

                definitionOffsets = ParseDefinitionOffsets(definitionOffsetsString);
                historyWatch = true;
                 
            }

            this.LoadDirectionArticle(direction, sense, definitionOffsets, historyWatch);
             
        }

        private void SpeakButton_Click(object sender, EventArgs e)
        {
            this.SpeakOnline();
            ManagerInstance.TelemetryClient.TrackEvent("DirectionArticle.SpeakButton.Clicked");
        }

        private void SpeakOnline()
        {
            OfflinePronounciation.SpeakAsync(tbWord.Text, this.Direction.SourceLanguageCode);
            if (tbWord.Text == "bug")
            {
                throw new InvalidProgramException("This is what bug means in software development");
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (this.ManagerInstance.History.HasPrevious)
            {
                var article = this.ManagerInstance.History.Previous;
                this.LoadDirectionArticle(ManagerInstance.GetDirection(article.DirectionId, LoadingState.Loaded), article.Sense, article.DefinitionOffsets, true);
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (this.ManagerInstance.History.HasNext)
            {
                var article = this.ManagerInstance.History.Next;
                this.LoadDirectionArticle(ManagerInstance.GetDirection(article.DirectionId, LoadingState.Loaded), article.Sense, article.DefinitionOffsets, true);
            }
        }
    }
}