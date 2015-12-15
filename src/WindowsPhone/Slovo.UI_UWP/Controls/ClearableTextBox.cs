using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Slovo.Controls
{

   // Currently is not used because of Telerik controls
   [TemplatePart(Name = "ClearButtonTemplate", Type = typeof(Windows.UI.Xaml.Controls.Button))]
   [TemplateVisualState(Name = "Normal", GroupName = "VisualStates")]
   [TemplateVisualState(Name = "Cleared", GroupName = "VisualStates")]
   public class ClearableTextBox : Windows.UI.Xaml.Controls.Control
   {
      Windows.UI.Xaml.Controls.Button clearBtn;
      Windows.UI.Xaml.Controls.TextBox editBox;

      // ClearButtonContent DP
      public object ClearButtonContent
      {
         get
         {
            return (object)GetValue(ClearButtonContentProperty);
         }
         set
         {
            SetValue(ClearButtonContentProperty, value);
         }
      }

      public static readonly Windows.UI.Xaml.DependencyProperty ClearButtonContentProperty = Windows.UI.Xaml.DependencyProperty.Register("ClearButtonContent", typeof(object), typeof(ClearableTextBox), new PropertyMetadata(null));

      // ClearButtonTemplate DP
      public ControlTemplate ClearButtonTemplate
      {
         get
         {
            return (ControlTemplate)GetValue(ClearButtonTemplateProperty);
         }
         set
         {
            SetValue(ClearButtonTemplateProperty, value);
         }
      }

      public static readonly Windows.UI.Xaml.DependencyProperty ClearButtonTemplateProperty = Windows.UI.Xaml.DependencyProperty.Register("ClearButtonTemplate", typeof(ControlTemplate), typeof(ClearableTextBox), new PropertyMetadata(null));

      // EditBoxTemplate DP
      public ControlTemplate EditBoxTemplate
      {
         get
         {
            return (ControlTemplate)GetValue(EditBoxTemplateProperty);
         }
         set
         {
            SetValue(EditBoxTemplateProperty, value);
         }
      }

      public static readonly Windows.UI.Xaml.DependencyProperty EditBoxTemplateProperty = Windows.UI.Xaml.DependencyProperty.Register("EditBoxTemplate", typeof(ControlTemplate), typeof(ClearableTextBox), new PropertyMetadata(null));

      // Text DP
      public string Text
      {
         get
         {
            return (string)GetValue(TextProperty);
         }
         set
         {
            SetValue(TextProperty, value);
         }
      }

      public static readonly Windows.UI.Xaml.DependencyProperty TextProperty = Windows.UI.Xaml.DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox), new PropertyMetadata(string.Empty));

      public ClearableTextBox()
      : base()
      {
         DefaultStyleKey = typeof(ClearableTextBox);
      }

      protected override void OnApplyTemplate()
      {
         base.OnApplyTemplate();
         // Hook up our click handler for the clear button.
         clearBtn = GetTemplateChild("clearBtn") as Windows.UI.Xaml.Controls.Button;
         clearBtn.Click += clrBtn_Click;
         editBox = GetTemplateChild("editBox") as Windows.UI.Xaml.Controls.TextBox;
         // Bind our text box to this instances Text property.
         Windows.UI.Xaml.Data.Binding b = new Windows.UI.Xaml.Data.Binding()
            {
               Path = new PropertyPath("Text")
            };
         b.Mode = Windows.UI.Xaml.Data.BindingMode.TwoWay;
         b.Source = this;
         editBox.SetBinding(Windows.UI.Xaml.Controls.TextBox.TextProperty, b);
      }

      private void clrBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
      {
         Windows.UI.Xaml.VisualStateManager.GoToState(this, "Normal", false); // An empty state, resets us for the cleared.

         Windows.UI.Xaml.VisualStateManager.GoToState(this, "Cleared", false);
         Text = string.Empty;
      }

   }

}