
namespace Slovo.Generator
{
    using Slovo.Core.Config;
    using Slovo.Core.Vocabularies;
    using Slovo.Generator.Direction;
    using Slovo.Generator.Vocabulary;
    using System;
    using System.Collections.Generic;

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
            WriteDirections();


            Console.WriteLine(DateTime.Now + ": Generator finished successfully. Press Enter to exit");
            Console.ReadLine();
        }

        private static void WriteVocabularies()
        {
            // new OjegovFormatter(pathToOjegov).WriteOutput();
            // new MullerFormatter(pathToMuller).WriteOutput();
            // new RuEngFormatter(pathToRuEn).WriteOutput();
            new Formatters.WordNet30Formatter().WriteOutput();
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
