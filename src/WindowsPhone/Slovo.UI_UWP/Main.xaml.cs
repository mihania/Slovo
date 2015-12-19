namespace Slovo.UI
{
    using Slovo.Core;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    public sealed partial class Main : Page
    {
        private const string SearchPivotName = "SearchPivotItem";

        /// <summary>
        /// Textbox.TextChanged fires twice. Known issue. 
        /// http://social.msdn.microsoft.com/Forums/en-US/windowsphone7series/thread/cb9b5544-a75e-4c9c-8475-0bf29e0bb5e3/
        /// </summary>
        private string previosTextValue = String.Empty;

        private int currentDirectionId = (int)DirectionId.English;

        private Dictionary<string, bool> loadedPivots = new Dictionary<string, bool>();

        public event EventHandler ApplicationBarClick;

        public Main()
        {
            InitializeComponent();
            // ToDo: Remove: verify RU culture
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            // localization resources
            HistoryLabel.Text = CommonResources.History;
            SearchLabel.Text = CommonResources.Search;
            VocabulariesLabel.Text = CommonResources.Vocabularies;
            this.DataContext = this;
        }

        private Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>> ManagerInstance { get { return Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance; } }

        #region #PhoneApplicationPage events

        /// <summary>
        /// Occurs when a FrameworkElement has been constructed and added to the object tree. Happens before layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>http://blogs.msdn.com/b/devdave/archive/2008/10/11/control-lifecycle.aspx
        /// public visibility on method is set to prevent obfuscation
        /// </remarks>
        public void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var current = Manager<PhoneStreamGetter, ObservableCollection<Vocabulary<PhoneStreamGetter>>>.Instance.CurrentDirection;
            if (current != null)
            {
                int topItemNumber = current.Cursor;
                
                // This check is required because of ArgumentOutOfRangeException on current[topItemNumber]
                if (topItemNumber >= 0 && current.List != null && topItemNumber < current.List.Count)
                {
                    this.tbSearch.Text = current[topItemNumber];
                    if (!string.IsNullOrEmpty(this.tbSearch.Text))
                    {
                        // selecting text, so it is easy to clear the text box
                        this.tbSearch.Select(0, this.tbSearch.Text.Length);

                        // scrolling is not restored when page is reloaded, need to do that manually.
                        this.ScrollIntoText(this.tbSearch.Text);
                    }
                }
            }

            this.ManageKeyboardVisibility();
            this.InitDirectionAsync();
        }

        private void MainPivot_LoadingPivotItem(Pivot sender, PivotItemEventArgs e)
        {
            if (!loadedPivots.ContainsKey(e.Item.Name))
            {
                if (e.Item.Name == "HistoryPivotItem")
                {
                    e.Item.Content = new HistoryControl();
                }
                else if (e.Item.Name == SettingsControl.ControlName)
                {
                    e.Item.Content = new SettingsControl(this);
                    ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[0]).Label = CommonResources.Apply;
                    ((AppBarButton)((CommandBar)BottomAppBar).PrimaryCommands[1]).Label = CommonResources.About;
                }
                this.loadedPivots[e.Item.Name] = true;
            }
            else
            {
                if (e.Item.Name == SettingsControl.ControlName)
                {
                    ((SettingsControl)e.Item.Content).OnLoading();
                }
            }
           ((CommandBar)BottomAppBar).Visibility = (e.Item.Name == SettingsControl.ControlName) ? Visibility.Visible : Visibility.Collapsed;
            this.ManageKeyboardVisibility();
        }

        private void tbSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                OnEnter();
            }
        }

        private void tbSearch_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            bool isVocabularyInitiated = false;
            if (tbSearch.Text.Length > 0)
            {
                char lastChar = tbSearch.Text[tbSearch.Text.Length - 1];
                if (currentDirectionId == (int)DirectionId.Russian)
                {
                    if (Common.IsEnglishLetter(lastChar))
                    {
                        currentDirectionId = (int)DirectionId.English;
                        this.InitDirectionAsync();
                        isVocabularyInitiated = true;
                    }
                }
                else if (currentDirectionId == (int)DirectionId.English)
                {
                    if (Common.IsRussianLetter(lastChar))
                    {
                        currentDirectionId = (int)DirectionId.Russian;
                        this.InitDirectionAsync();
                        isVocabularyInitiated = true;
                    }
                }
            }

            if (!isVocabularyInitiated && ManagerInstance.CurrentDirection == null)
            {
                this.InitDirectionAsync();
            }

            if (ManagerInstance.CurrentLoadingState == LoadingState.Loaded)
            {
                this.ScrollIntoText(tbSearch.Text);
            }
        }

        private void wordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.wordList.SelectedIndex != -1)
            {
                this.tbSearch.Text = ManagerInstance.CurrentDirection[this.wordList.SelectedIndex];
                ManagerInstance.CurrentDirection.Cursor = this.wordList.SelectedIndex;
                this.OpenWord(wordList.SelectedIndex);
            }
        }

        #endregion

        internal bool InitDirectionSync()
        {
            return InitDirection(false);
        }

        private bool InitDirectionAsync()
        {
            return InitDirection(true);
        }

        /// <summary>
        /// Init vocabulary
        /// </summary>
        /// <returns>True, if vocabulary loaded, false if it is loading</returns>
        private bool InitDirection(bool async)
        {
            bool result = false;
            lock (this.previosTextValue)
            {
                var direction = this.ManagerInstance.Configuration.Directions.GetDirectionById(this.currentDirectionId);
                if (direction.LoadingState == LoadingState.Loaded)
                {
                    ManagerInstance.CurrentDirection = direction;
                    wordList.ItemsSource = new VirtualWordList(ManagerInstance.CurrentDirection.List);
                    this.ManageKeyboardVisibility();
                    this.ScrollIntoText(tbSearch.Text);
                    result = true;
                }
                else
                {
                    if (async)
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                        bw.RunWorkerAsync();
                    }
                    else
                    {
                        this.bw_DoWork(this, null);
                        this.bw_RunWorkerCompleted(this, null);
                    }
                }
            }

            return result;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wordList.ItemsSource = new VirtualWordList(ManagerInstance.CurrentDirection.List);
            this.ManageKeyboardVisibility();
        }

        private void ManageKeyboardVisibility()
        {
            if (this.MainPivot.SelectedIndex == 0)
            {
                this.tbSearch.Focus(FocusState.Programmatic);
            }
            else
            {
                // just to loose focus
                this.HistoryPivotItem.Focus(FocusState.Programmatic);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ManagerInstance.CurrentDirection = this.ManagerInstance.Configuration.Directions[this.currentDirectionId];
            ManagerInstance.CurrentDirection.Deserialize(LoadingState.Loaded);
            ManagerInstance.CurrentDirection.SupportPronounciation = Slovo.UI.Core.Pronounciation.OfflinePronounciation.SupportPronounciation(ManagerInstance.CurrentDirection.SourceLanguageCode);
        }

        private void ScrollIntoText(string text)
        {
            if (wordList.Items.Count > 0)
            {
                previosTextValue = text;
                text = text.Trim();
                int pos = ManagerInstance.CurrentDirection.SetPosition(text);
                if (pos < 0)
                {
                    pos = 0;
                }
                else if (pos >= wordList.Items.Count)
                {
                    pos = wordList.Items.Count - 1;
                }

                wordList.ScrollIntoView(wordList.Items[pos], ScrollIntoViewAlignment.Leading);
            }
        }

        /// <summary>
        /// Event handler that is executed when 'Enter' key is pressed down
        /// </summary>
        private void OnEnter()
        {
            if (ManagerInstance.CurrentLoadingState == LoadingState.Loaded)
            {
                if (ManagerInstance.CurrentDirection.IsWordFound)
                {
                    this.OpenWord(ManagerInstance.CurrentDirection.Cursor);
                }
            }
        }

        private void OpenWord(int directionArticleId)
        {
            this.Frame.Navigate(typeof(DirectionArticle2), new DirectionArticleNavigateParams() { DirectionId = ManagerInstance.CurrentDirection.Id, DirectionArticleId = directionArticleId });
        }

        private void ImageInfoClicked(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            //if (this.ApplicationBarClick != null)
            //{
            //    this.ApplicationBarClick(ApplicationBarButton.Apply, null);
            //}
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }
    }
}
