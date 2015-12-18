namespace Slovo.Generator
{
    using System;
    using System.Xml;

    class XamlFormatter : ITypeFormatter
    {
        public string OutputPath
        {
            get
            {
                return @"..\..\..\Slovo.UI\Data";
            }
        }

        public string AlternateColorBegin
        {
            get
            {
                return "<Span Foreground=\"Gray\">";
            }
        }

        public string AlternateColorEnd
        {
            get
            {
                return "</Span>";
            }
        }

        public string AccentColorBegin
        {
            get
            {
                return "<Span Foreground=\"Red\">";
            }
        }

        public string NewLine
        {
            get { return "\n"; }
        }

        public string Escape(string text)
        {
            return string.IsNullOrEmpty(text) ? text : text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        public bool IsValid(string definition, string word)
        {
            bool result = false;
            try
            {
                string xml = "<doc>" + definition + "</doc>";
                XmlDocument doc = new XmlDocument();

                // this call has to fail if xml is wrong
                doc.LoadXml(xml);

                result = true;
            }
            catch (XmlException ex)
            {
                throw new ApplicationException(word + ": inccorect translation xml: " + ex.Message, ex);
            }

            return result;
        }
    }
}
