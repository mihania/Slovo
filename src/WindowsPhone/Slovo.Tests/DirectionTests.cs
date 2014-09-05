namespace Slovo.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Slovo.Core.Directions;
    using Slovo.Generator.Direction;
    using System.IO;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DirectionTests
    {
        private class TestFileStreamGetter : FileStreamGetter {
            protected override string MapFileName(string fileName)
            {
                string path = Directory.GetCurrentDirectory();
                string fileSystemName = Path.Combine(path, @"..\..\..\Slovo\", fileName);
                return fileSystemName;
            }
        }
    }
}
