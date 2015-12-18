using System;

namespace Slovo.Controls
{
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using Slovo.Core;

    public class ScrollableTextBlock : Control
    {
        private StackPanel stackPanel;
        private string SampleTextToMeasure = " ";
        private Size? SampleTextSize;

        public ScrollableTextBlock()
        {
            // Get the style from generic.xaml
            this.DefaultStyleKey = typeof(ScrollableTextBlock);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ScrollableTextBlock),
                new PropertyMetadata("ScrollableTextBlock", OnTextPropertyChanged));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                value = value + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                SetValue(TextProperty, value);
            }
        }


        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollableTextBlock source = (ScrollableTextBlock)d;
            string value = (string)e.NewValue;
            source.ParseText(value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.stackPanel = this.GetTemplateChild("StackPanel") as StackPanel;
            this.ParseText(this.Text);
        }

        private void ParseText(string value)
        {
            if (this.stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks
            this.stackPanel.Children.Clear();

            using (StringReader reader = new StringReader(value))
            {
                string line;
                int lineCount = 0;
                StringBuilder sb = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    if (lineCount == 0) 
                    {
                        sb.Append("<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph xml:space=\"preserve\">");
                    }


                    sb.Append(line);
                    lineCount++;

                    if (lineCount == Core.Common.MaxLinesPerTextBlock)
                    {
                        RichTextBox textBlock = this.GetTextBlock();
                        sb.Append("</Paragraph></Section>");
                        textBlock.Xaml = sb.ToString();
                        this.stackPanel.Children.Add(textBlock);
                        sb.Clear();
                        lineCount = 0;
                    }
                    else
                    {
                        sb.Append('\n');
                    }
                }
                
                if (sb.Length != 0)
                {
                    RichTextBox textBlock = this.GetTextBlock();
                    sb.Append("</Paragraph></Section>");
                    textBlock.Xaml = sb.ToString();
                    this.stackPanel.Children.Add(textBlock);
                    sb.Clear();
                }
            }
        }

        private RichTextBox GetTextBlock()
        {
            RichTextBox textBlock = new RichTextBox();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            textBlock.FontWeight = this.FontWeight;
            textBlock.Foreground = this.Foreground;
            textBlock.Margin = new Thickness(0, 0, MeasureText(this.SampleTextToMeasure).Width, 0);
            return textBlock;
        }

        private Size MeasureText(string value)
        {
            if (value == SampleTextToMeasure && this.SampleTextSize.HasValue)
            {
                return this.SampleTextSize.Value;
            }
            else
            {
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.FontSize = this.FontSize;
                textBlock.FontFamily = this.FontFamily;
                textBlock.FontWeight = this.FontWeight;

                textBlock.Text = value;

                Size result = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
                if (value == SampleTextToMeasure)
                {
                    this.SampleTextSize = result;
                }

                return result;
            }
        }
    }
}
