namespace Slovo.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Generator.Formatters;
    using Slovo.Generator;

    [TestClass]
    public class WordNet30FormatterTest
    {
        [TestClass]
        public class ItalianQuotesTest
        {
            [TestMethod]
            public void TestEmptyString()
            {
                Assert.AreEqual(string.Empty,  new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(string.Empty));
            }

            [TestMethod]
            public void TestNoQuotes()
            {
                string s = ReplaceQuotes("abc");
                Assert.AreEqual(s, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInvalidNumber()
            {
                string s = ReplaceQuotes("a\"b\"\"c");
                Assert.AreEqual(s, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            [TestMethod]
            public void TestCornerQuotes()
            {
                string s = ReplaceQuotes("\"ac\"");
                Assert.AreEqual(new XamlFormatter().AlternateColorBegin + "ac" + new XamlFormatter().AlternateColorEnd, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInnerTogetherQuotes()
            {
                string s = ReplaceQuotes("\"ab\"\"cd\"");
                Assert.AreEqual(new XamlFormatter().AlternateColorBegin + "ab" + new XamlFormatter().AlternateColorEnd + new XamlFormatter().AlternateColorBegin + "cd" + new XamlFormatter().AlternateColorEnd, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            [TestMethod]
            public void TestInnerQuotes()
            {
                string s = ReplaceQuotes("\"ab\"ef\"cd\"");
                Assert.AreEqual(new XamlFormatter().AlternateColorBegin + "ab" + new XamlFormatter().AlternateColorEnd + "ef" + new XamlFormatter().AlternateColorBegin + "cd" + new XamlFormatter().AlternateColorEnd, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            [TestMethod]
            public void TestNewLine()
            {
                string s = ReplaceQuotes("\"ab\";\"cd\"");
                Assert.AreEqual(new XamlFormatter().AlternateColorBegin + "ab" + new XamlFormatter().AlternateColorEnd + "\n" + new XamlFormatter().AlternateColorBegin + "cd" + new XamlFormatter().AlternateColorEnd, new WordNet30Formatter(new XamlFormatter()).ItalianQuotes(s));
            }

            private string ReplaceQuotes(string s)
            {
                return s.Replace("\"", WordNet30Formatter.QuoteReplacement);
            }
        }

        [TestClass]
        public class SynsetTest
        {
            [TestMethod]
            public void TestTake()
            {
                string line =
                    "02267989 40 v 03 take 6 occupy 8 use_up 2 004 $ 01157517 v 0000 @ 01158572 v 0000 + 15141486 n 0201 ~ 02268246 v 0000 02 + 08 00 + 11 00 | require (time or space); \"It took three hours to get to work this morning\"; \"This event occupied a very short time";
                WordNet30Formatter.Synset s = new WordNet30Formatter(new XamlFormatter()).Parse("take", line);
                Assert.AreEqual(2, s.Synonims.Count, "Synonims count is incorect");
                Assert.AreEqual("occupy", s.Synonims[0], "occupy");
                Assert.AreEqual("use up", s.Synonims[1], "use up");
            }
        }
    }
}
