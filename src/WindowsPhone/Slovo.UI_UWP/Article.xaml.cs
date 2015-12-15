namespace Slovo.UI
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;

    // https://blogs.msdn.microsoft.com/tess/2013/05/13/displaying-html-content-in-a-richtextblock/
    public partial class Article : Windows.UI.Xaml.Controls.UserControl
    {

        public Article()
        {
            InitializeComponent();
        }

        internal string Definition
        {
            set
            {
                string content = @"{\rtf1\ansi\deff0
{\colortbl;\red0\green0\blue0;\red255\green0\blue0;}
This line is the default color\line
\cf2
This line is red\line
\cf1
This line is the default color
}";
                tbDefinition.IsReadOnly = false;
                tbDefinition.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, value);
                tbDefinition.IsReadOnly = true;
            }
        }
    }
}