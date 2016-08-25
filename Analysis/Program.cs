using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string directory = @"C:\Dev\CSharpCorpusMini\";
            var projectCount = Directory.EnumerateDirectories(directory).Count();

            var analyses = Analyse(CSharpFilesIn(directory));

            Console.WriteLine("Searching in: " + directory);
            Console.WriteLine("Projects: " + projectCount);
            Console.WriteLine("Files: " + analyses.Count);
            Console.WriteLine("Classes: " + analyses.Sum(a => a.ClassCount));
            Console.WriteLine("Methods: " + analyses.Sum(a => a.MethodCount));
            Console.WriteLine("Virtual Methods: " + analyses.Sum(a => a.VirtualMethodCount));
            Console.WriteLine("Override Methods: " + analyses.Sum(a => a.OverrideMethodCount));

            Console.ReadLine();
        }

        private static List<FileAnalysis> Analyse(List<string> files)
        {
            PrintToOutput(files.Count + " files");
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
