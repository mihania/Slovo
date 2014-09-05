namespace Slovo.UI
{
    using System.Windows.Controls;

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
                this.tbDefinition.Text = value;
            }
        }
    }
}