
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

            Write81();
            Write10();


            Console.WriteLine(DateTime.Now + ": Generator finished successfully. Press Enter to exit");
            Console.ReadLine();
        }

        private static void Write81()
        {
            new OjegovFormatter(pathToOjegov, new XamlFormatter()).WriteOutput();
            new MullerFormatter(pathToMuller, new XamlFormatter()).WriteOutput();
            new RuEngFormatter(pathToRuEn, new XamlFormatter()).WriteOutput();
            new Formatters.WordNet30Formatter(new XamlFormatter()).WriteOutput();
            WriteDirections81();
        }

        private static void Write10()
        {
            new OjegovFormatter(pathToOjegov, new RtfFormatter()).WriteOutput();
            new MullerFormatter(pathToMuller, new RtfFormatter()).WriteOutput();
            new RuEngFormatter(pathToRuEn, new RtfFormatter()).WriteOutput();
            new Formatters.WordNet30Formatter(new RtfFormatter()).WriteOutput();
            WriteDirections10();
        }

        private static void WriteDirections81()
        {
            // var configuration = new Configuration<FileStreamGetter, List<Vocabulary<FileStreamGetter>>();
            var configuration = Configuration<FileStreamGetter81, List<Vocabulary<FileStreamGetter81>>>.LoadConfiguration();
            foreach (var direction in configuration.Directions)
            {
                direction.Serialize();
            }
        }

        private static void WriteDirections10()
        {
            // var configuration = new Configuration<FileStreamGetter, List<Vocabulary<FileStreamGetter>>();
            var configuration = Configuration<FileStreamGetter10, List<Vocabulary<FileStreamGetter10>>>.LoadConfiguration();
            foreach (var direction in configuration.Directions)
            {
                direction.Serialize();
            }
        }

    }
}
