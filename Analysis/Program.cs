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
            const string directory = @"C:\Dev\CSharpCorpus\coreclr";

            var files = CSharpFilesIn(directory);

            var analyses = files.Select(file => new FileAnalysis(file)).ToList();

            Console.WriteLine("Method Count: " + analyses.Sum(a => a.MethodCount));
            Console.WriteLine("Virtual Method Count: " + analyses.Sum(a => a.VirtualMethodCount));

            Console.ReadLine();
        }

        private static IEnumerable<string> CSharpFilesIn(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories);
        }
    }
}
