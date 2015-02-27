namespace Slovo.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Generator.Formatters;

    [TestClass]
    public class WordNet30FormatterTest
    {
        [TestClass]
        public class ItalianQuotesTest
        {
            [TestMethod]
            public void TestEmptyString()
            {
                Assert.AreEqual(string.Empty,  WordNet30Formatter.ItalianQuotes(string.Empty));
            }

            [TestMethod]
            public void TestNoQuotes()
            {
                string s = ReplaceQuotes("abc");
                Assert.AreEqual(s, WordNet30Formatter.ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInvalidNumber()
            {
                string s = ReplaceQuotes("a\"b\"\"c");
                Assert.AreEqual(s, WordNet30Formatter.ItalianQuotes(s));
            }

            [TestMethod]
            public void TestCornerQuotes()
            {
                string s = ReplaceQuotes("\"ac\"");
                Assert.AreEqual(WordNet30Formatter.StartItalicQuote + "ac" + WordNet30Formatter.EndItalicQuote, WordNet30Formatter.ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInnerTogetherQuotes()
            {
                string s = ReplaceQuotes("\"ab\"\"cd\"");
                Assert.AreEqual(WordNet30Formatter.StartItalicQuote + "ab" + WordNet30Formatter.EndItalicQuote + WordNet30Formatter.StartItalicQuote + "cd" + WordNet30Formatter.EndItalicQuote, WordNet30Formatter.ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInnerQuotes()
            {
                string s = ReplaceQuotes("\"ab\"ef\"cd\"");
                Assert.AreEqual(WordNet30Formatter.StartItalicQuote + "ab" + WordNet30Formatter.EndItalicQuote + "ef" + WordNet30Formatter.StartItalicQuote + "cd" + WordNet30Formatter.EndItalicQuote, WordNet30Formatter.ItalianQuotes(s));
            }

            [TestMethod]
            public void TestNewLine()
            {
                string s = ReplaceQuotes("\"ab\";\"cd\"");
                Assert.AreEqual(WordNet30Formatter.StartItalicQuote + "ab" + WordNet30Formatter.EndItalicQuote + "\n" + WordNet30Formatter.StartItalicQuote + "cd" + WordNet30Formatter.EndItalicQuote, WordNet30Formatter.ItalianQuotes(s));
            }

            private string ReplaceQuotes(string s)
            {
                return s.Replace("\"", WordNet30Formatter.QuoteReplacement);
            }
        }
    }
}
