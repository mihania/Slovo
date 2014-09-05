using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Slovo.Controls
{
    // Currently is not used because of Telerik controls
    [TemplatePart(Name = "ClearButtonTemplate", Type = typeof(Button))]
    [TemplateVisualState(Name = "Normal", GroupName = "VisualStates")]
    [TemplateVisualState(Name = "Cleared", GroupName = "VisualStates")]
    public class ClearableTextBox : Control
    {
        Button clearBtn;
        TextBox editBox;

        // ClearButtonContent DP
        public object ClearButtonContent
        {
            get { return (object)GetValue(ClearButtonContentProperty); }
            set { SetValue(ClearButtonContentProperty, value); }
        }

        public static readonly DependencyProperty ClearButtonContentProperty =
            DependencyProperty.Register("ClearButtonContent", typeof(object), typeof(ClearableTextBox), new PropertyMetadata(null));

        // ClearButtonTemplate DP
        public ControlTemplate ClearButtonTemplate
        {
            get { return (ControlTemplate)GetValue(ClearButtonTemplateProperty); }
            set { SetValue(ClearButtonTemplateProperty, value); }
        }

        public static readonly DependencyProperty ClearButtonTemplateProperty =
            DependencyProperty.Register("ClearButtonTemplate", typeof(ControlTemplate), typeof(ClearableTextBox), new PropertyMetadata(null));

        // EditBoxTemplate DP
        public ControlTemplate EditBoxTemplate
        {
            get { return (ControlTemplate)GetValue(EditBoxTemplateProperty); }
            set { SetValue(EditBoxTemplateProperty, value); }
        }

        public static readonly DependencyProperty EditBoxTemplateProperty =
            DependencyProperty.Register("EditBoxTemplate", typeof(ControlTemplate), typeof(ClearableTextBox), new PropertyMetadata(null));

        // Text DP
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty));

        public ClearableTextBox()
            : base()
        {
            DefaultStyleKey = typeof(ClearableTextBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Hook up our click handler for the clear button.
            clearBtn = GetTemplateChild("clearBtn") as Button;
            clearBtn.Click += clrBtn_Click;

            editBox = GetTemplateChild("editBox") as TextBox;

            // Bind our text box to this instances Text property.
            Binding b = new Binding("Text");
            b.Mode = BindingMode.TwoWay;
            b.Source = this;
            editBox.SetBinding(TextBox.TextProperty, b);

        }

        private void clrBtn_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false); // An empty state, resets us for the cleared.
            VisualStateManager.GoToState(this, "Cleared", false);
            Text = string.Empty;
        }
    }
}
