using System.IO;

namespace Slovo.Generator
{
    using System;
    using System.Collections.Generic;
    using Slovo.Generator.Pronounciation;
    using Slovo.Core;
    using Slovo.Generator.Formatters;
    using Slovo.Generator.Direction;
    using Slovo.Core.Directions;
    using Slovo.Generator.Vocabulary;
    using Slovo.Core.Config;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Slovo.Core.Directions;
    using System.Collections.Generic;
    using Slovo.Core.Vocabularies;

    class Program
    {
        private const string pathToMuller = @"..\..\InputData\Mueller.koi";
        private const string pathToRuEn = @"..\..\InputData\RuEn.txt";
        private const string pathToOjegov = @"..\..\InputData\ojegov.def";
        private const string pathToMullerXml = @"..\..\OutputData\Mueller.xml";
        private const string pathToRuEnXml = @"..\..\OutputData\RuEn.xml";
        private const string pathToEnTranscription = @"";
        
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now + ": Generator started successfully");
            
            WriteVocabularies();
            
            // WriteDirections();


            Console.WriteLine(DateTime.Now + ": Generator finished successfully. Press Enter to exit");
            Console.ReadLine();
        }

        private static void WriteVocabularies()
        {
            new OjegovFormatter(pathToOjegov).WriteOutput();
            // new MullerFormatter(pathToMuller).WriteOutput();
            // new RuEngFormatter(pathToRuEn).WriteOutput();
            // new Formatters.WordNet3_0Formatter().WriteOutput();
        }

        private static void WriteDirections()
        {
            // var configuration = new Configuration<FileStreamGetter, List<Vocabulary<FileStreamGetter>>();
            var configuration = Configuration<FileStreamGetter, List<Vocabulary<FileStreamGetter>>>.LoadConfiguration();
            foreach (var direction in configuration.Directions)
            {
                direction.Serialize();
            }
        }
    }
}
