namespace Slovo.UI.Controls
{
    using Windows.UI.Xaml.Controls;

    // https://blogs.msdn.microsoft.com/tess/2013/05/13/displaying-html-content-in-a-richtextblock/
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
                tbDefinition.IsReadOnly = false;
                tbDefinition.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, value);
                tbDefinition.IsReadOnly = true;
            }
        }
    }
}