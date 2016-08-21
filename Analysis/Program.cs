using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using ConsoleApplication1;

namespace CSharpAnalysis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var fileStream = new StreamReader(@"C:\Dev\CSharpCorpus\AutoMapper\src\AutoMapper\AutoMapperMappingException.cs"))
            {
                var inputStream = new AntlrInputStream(fileStream);

                var lexer = new CSharpLexer(inputStream);
                var commonTokenStream = new CommonTokenStream(lexer);
                var parser = new CSharpParser(commonTokenStream);

                parser.RemoveErrorListeners();
                parser.AddErrorListener(new BaseErrorListener());

                var builtParseTree = parser.BuildParseTree;
                Console.WriteLine(parser.compilation_unit());
                Console.ReadLine();
            }
        }
    }
}
