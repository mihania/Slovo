using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slovo.Generator;

namespace Slovo.Tests
{
    [TestClass]
    public class ArticleTest
    {
        [TestMethod]
        public void Test1()
        {
            var a1 = new Slovo.Generator.Article("reeded", "t1");
            var a2 = new Slovo.Generator.Article("re-education", "t2");
            var a3 = new Slovo.Generator.Article("reedy", "t3");

            var list = new List<Article>() {a1, a2, a3};
            list.Sort();
            Assert.AreEqual(list[0].Word, "re-education");
            Assert.AreEqual(list[1].Word, "reeded");
            Assert.AreEqual(list[2].Word, "reedy");
        }
    }
}
