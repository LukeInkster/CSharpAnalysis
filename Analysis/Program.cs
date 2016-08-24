using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CSharpAnalysis;

namespace CSharpAnalysis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string directory = @"C:\Dev\CSharpCorpus\AutoMapper";

            var files = CSharpFilesIn(directory);

            foreach (var file in files)
            {
                Analyse(file);
            }
        }

        private static void Analyse(string file)
        {
            using (var fileStream = new StreamReader(file))
            {
                var inputStream = new AntlrInputStream(fileStream);
                var lexer = new CSharpLexer(inputStream);
                var commonTokenStream = new CommonTokenStream(lexer);
                var parser = new CSharpParser(commonTokenStream);

                parser.RemoveErrorListeners();
                parser.AddErrorListener(new BaseErrorListener());

                var visitor = new CSharpVisitor();
                
                visitor.VisitCompilation_unit(parser.compilation_unit());

                Console.WriteLine("Methods: " + visitor.MethodCount);
                Console.WriteLine("Virtual Methods: " + visitor.VirtualMethodCount);

                Console.ReadLine();
            }
        }

        private static IEnumerable<string> CSharpFilesIn(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories);
        }
    }
}
