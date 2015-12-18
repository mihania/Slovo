namespace Slovo.UI.Controls
{
    using Windows.UI.Xaml.Controls;

    public partial class Article : UserControl
    {

        public Article()
        {
            InitializeComponent();
        }

        internal string Definition
        {
            set
            {
                this.Viewer.IsReadOnly = false;
                this.Viewer.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, value);
                this.Viewer.IsReadOnly = true;
            }
        }
    }
}