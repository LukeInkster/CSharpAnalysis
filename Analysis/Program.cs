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
            const string corpusDirectory = @"C:\Dev\CSharpCorpusMini\";

            var projectDirectories = Directory
                .EnumerateDirectories(corpusDirectory);

            var projects = projectDirectories
                .Select(dir => new Project(dir))
                .ToList();

            Console.WriteLine("Searching in: " + corpusDirectory);
            Console.WriteLine("Projects: " + projects.Count);
            Console.WriteLine("Files: " + projects.Sum(p => p.FileCount));
            Console.WriteLine("Classes: " + projects.Sum(p => p.ClassCount));
            Console.WriteLine("Extended Classes: " + projects.Sum(p => p.ExtendingClassCount));
            Console.WriteLine("Methods: " + projects.Sum(p => p.MethodCount));
            Console.WriteLine("Virtual Methods: " + projects.Sum(p => p.VirtualMethodCount));
            Console.WriteLine("Override Methods: " + projects.Sum(p => p.OverrideMethodCount));

            Console.ReadLine();
        }
    }
}
