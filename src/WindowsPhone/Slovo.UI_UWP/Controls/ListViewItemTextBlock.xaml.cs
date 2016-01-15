using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Slovo
{
    public sealed partial class ListViewItemTextBlock : UserControl
    {
        public ListViewItemTextBlock()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ListViewItemTextBlock), new PropertyMetadata(""));

        public string Text
        {
            set
            {
                this.textBlock.Text = value;
            }
        }
    }
}
