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
            using (var fileStream = new StreamReader(@"C:\Dev\test.cs"))
            {
                var inputStream = new AntlrInputStream(fileStream);
                var lexer = new CSharpLexer(inputStream);
                var commonTokenStream = new CommonTokenStream(lexer);
                var parser = new CSharpParser(commonTokenStream);

                parser.RemoveErrorListeners();
                parser.AddErrorListener(new BaseErrorListener());

                var visitor = new CSharpVisitor();//new CSharpParserBaseVisitor<int>();

                var builtParseTree = parser.BuildParseTree;
                var tree = parser.Context;
                //visitor.Visit(parser.Context);
                var compilationUnit = parser.compilation_unit();
                visitor.VisitCompilation_unit(compilationUnit);

                Console.WriteLine(compilationUnit);
                Console.WriteLine(tree);
                Console.WriteLine(parser.compilation_unit());
                //Console.WriteLine(visitor.Visit(tree));
                Console.ReadLine();
            }
        }
    }
}
