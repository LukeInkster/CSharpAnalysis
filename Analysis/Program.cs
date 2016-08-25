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
            const string directory = @"C:\Dev\CSharpCorpus";

            var analyses = Analyse(CSharpFilesIn(directory));

            Console.WriteLine("Method Count: " + analyses.Sum(a => a.MethodCount));
            Console.WriteLine("Virtual Method Count: " + analyses.Sum(a => a.VirtualMethodCount));

            Console.ReadLine();
        }

        private static List<FileAnalysis> Analyse(List<string> files)
        {
            PrintToOutput(files.Count() + " files");
            var analyses = new List<FileAnalysis>();
            foreach (var file in files)
            {
                analyses.Add(new FileAnalysis(file));
                if (analyses.Count%1000 == 0)
                {
                    PrintToOutput(analyses.Count + "Files analysed");
                }
            }
            return analyses;
        }

        private static List<string> CSharpFilesIn(string dir)
        {
            return Directory.EnumerateFiles(
                path: dir,
                searchPattern: "*.cs", 
                searchOption: SearchOption.AllDirectories)
                .ToList();
        }

        public static void PrintToOutput(string s)
        {
            System.Diagnostics.Debug.Write("\n" + s);
        }
    }
}
