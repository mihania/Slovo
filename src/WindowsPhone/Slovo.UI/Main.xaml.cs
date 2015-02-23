    namespace Slovo.UI
{
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Slovo.Core;
    using Slovo.Core.Directions;
    using Slovo.Core.Vocabularies;
    using Slovo.Resources;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.ApplicationInsights.DataContracts;

    public partial class Main : PhoneApplicationPage
    {
        private const string SearchPivotName = "SearchPivotItem";
        /// <summary>
        /// The varible to distringuish list box selected index was changed because of user selection or probrammatic selection.
        /// </summary>
        private bool isUserClicked = true;

        /// <summary>
        /// Textbox.TextChanged fires twice. Known issue. 
        /// http://social.msdn.microsoft.com/Forums/en-US/windowsphone7series/thread/cb9b5544-a75e-4c9c-8475-0bf29e0bb5e3/
        /// </summary>
        private string previosTextValue = String.Empty;
        
        private int currentDirectionId = (int)DirectionId.English;
        
        private Dictionary<string, bool> loadedPivots = new Dictionary<string,bool>();

        public event EventHandler ApplicationBarClick;

        public Main()
        {
            InitializeComponent();
            
            // ToDo: Remove: verify RU culture
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            
            // Events need to be attached in code behind to allow obfuscator correctly obfuscate method names.
            // Obfuscator does not obfuscate xaml
            tbSearch.KeyDown += new KeyEventHandler(tbSearch_KeyDown);
            tbSearch.KeyUp += new KeyEventHandler(tbSearch_KeyUp);
            wordList.SelectionChanged += new SelectionChangedEventHandler(wordList_SelectionChanged);
            MainPivot.LoadingPivotItem += new EventHandler<PivotItemEventArgs>(MainPivot_LoadingPivotItem);
            MainPivot.UnloadingPivotItem += new EventHandler<PivotItemEventArgs>(MainPivot_UnloadingPivotItem);
            
            // localization resources
            HistoryLabel.Text = CommonResources.History;
            SearchLabel.Text = CommonResources.Search;
            VocabulariesLabel.Text = CommonResources.Vocabularies;
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
                    }
                }
            }

            this.ManageKeyboardVisibility();
            this.InitDirectionAsync();
        }

        private void MainPivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
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
                    ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = CommonResources.Apply;
                    ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = CommonResources.About;
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

            ApplicationBar.IsVisible = (e.Item.Name == SettingsControl.ControlName);

            this.ManageKeyboardVisibility();
        }

        void MainPivot_UnloadingPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item.Name == SettingsControl.ControlName)
            {
                // this code is not used now
            }
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                OnEnter();
            }
        }

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
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
                this.SelectItem(tbSearch.Text);
            }
        }

        private void wordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isUserClicked && this.wordList.SelectedIndex != -1)
            {
                this.tbSearch.Text = ManagerInstance.CurrentDirection[this.wordList.SelectedIndex];
                
                // this.tbSearch.SelectionStart = this.tbSearch.Text.Length;
                this.OpenWord(wordList.SelectedIndex);
            }

            //ToDo: understand whether this needed.
            this.isUserClicked = true;
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
                    this.SelectItem(tbSearch.Text);
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
            if (this.MainPivot.SelectedIndex == 0)
            {
                this.SelectItem(tbSearch.Text);
            }

            this.ManageKeyboardVisibility();
        }

        private void ManageKeyboardVisibility() 
        {
            if (this.MainPivot.SelectedIndex == 0)
            {
                this.tbSearch.Focus();
            }
            else
            {
                // just to loose focus
                this.HistoryPivotItem.Focus();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ManagerInstance.CurrentDirection = this.ManagerInstance.Configuration.Directions[this.currentDirectionId];
            ManagerInstance.CurrentDirection.Deserialize(LoadingState.Loaded);
            ManagerInstance.CurrentDirection.SupportPronounciation = Slovo.UI.Core.Pronounciation.OfflinePronounciation.SupportPronounciation(ManagerInstance.CurrentDirection.SourceLanguageCode);
        }

        private void SelectItem(string searchText)
        {
            // move selected item to the top
            // ToDo: Get rid of magic 11 number. 11 - number of items shown on the screen in listbox
            const int viewItemsCount = 10;
            if (!previosTextValue.Equals(searchText) && wordList.Items.Count > 0)
            {

                previosTextValue = searchText;
                searchText = searchText.Trim();

                int realPos = ManagerInstance.CurrentDirection.GetPosition(searchText);
                int newIndex;
                if (realPos < viewItemsCount)
                {
                    newIndex = realPos;
                }
                else
                {
                    newIndex = realPos + viewItemsCount;
                }

                if (newIndex < 0)
                {
                    newIndex = 0;
                }
                else if (newIndex >= wordList.Items.Count)
                {
                    newIndex = wordList.Items.Count - 1;
                }

                // The selected index is the bottom one.
                // When we need to select programmatically upper item, list is not shift, we need to shift it manually
                int prevSelected = wordList.SelectedIndex;
                if (wordList.SelectedIndex == -1 ||
                     (prevSelected - newIndex > 0 && prevSelected - newIndex < viewItemsCount))
                {
                    int tempNewIndex = newIndex - viewItemsCount;
                    if (tempNewIndex >= 0)
                    {
                        this.SetSelectedIndex(tempNewIndex);
                    }
                }

                this.SetSelectedIndex(newIndex);
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
                    this.SetSelectedIndex(ManagerInstance.CurrentDirection.Cursor);
                    this.OpenWord(ManagerInstance.CurrentDirection.Cursor);
                }
                else
                {
                    EventTelemetry telemetry = new EventTelemetry("WordNotFound");
                    telemetry.Properties["word"] = this.tbSearch.Text;
                    ManagerInstance.TelemetryClient.TrackEvent(telemetry);
                }
            }
        }

        private void OpenWord(int wordIndex)
        {
            var direction = ManagerInstance.CurrentDirection;
            ManagerInstance.CurrentDirection.Cursor = this.wordList.SelectedIndex;
            if (Settings.IsTrial && ! direction.IsSupportedInTrial(wordIndex))
            {
                NavigationService.Navigate(new Uri(Buy.GetPageUrl(direction[wordIndex]), UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri(Slovo.DirectionArticle.GetPageUrl(ManagerInstance.CurrentDirection.Id, wordIndex), UriKind.Relative));
            }
        }

        private void ImageInfoClicked(object sender, ManipulationStartedEventArgs e)
        {
            NavigationService.Navigate(new Uri(About.GetPageUrl(), UriKind.Relative));
        }

        private void SetSelectedIndex(int index)
        {
            this.isUserClicked = false;
            this.wordList.SelectedIndex = index;
            this.wordList.UpdateLayout();
            this.isUserClicked = true;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (this.ApplicationBarClick != null)
            {
                this.ApplicationBarClick(ApplicationBarButton.Apply, e);
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri(About.GetPageUrl(), UriKind.Relative));
        }
    }

    public enum ApplicationBarButton 
    {
        Apply = 0
    }
}