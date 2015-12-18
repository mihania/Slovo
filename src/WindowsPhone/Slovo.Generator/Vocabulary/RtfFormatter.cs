namespace Slovo.Generator
{
    /// <summary>
    /// Currently, RichEditBox in UWP 10 has a bug for showing text in a different color.
    /// Therefore using bold and italic instead of colors for the text.
    /// </summary>
    class RtfFormatter : ITypeFormatter
    {
        public string OutputPath
        {
            get { return @"..\..\..\Slovo.UI_UWP\Data"; }
        }

        public string AlternateColorBegin
        {
            get { return @"{\i "; }
        }

        public string AlternateColorEnd
        {
            get { return @"}"; }
        }

        public string AccentColorBegin
        {
            get { return @"{\b "; }
        }

        public string NewLine
        {
            get { return @"\line "; }
        }

        public string Escape(string text)
        {
            return text;
        }

        public bool IsValid(string definition, string word)
        {
            return true;
        }
    }
}
