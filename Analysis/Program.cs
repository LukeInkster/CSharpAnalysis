using System;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Program
    {
        private const string CorpusDirectory = @"C:\Dev\CSharpCorpusMini\";

        public static void Main(string[] args)
        {
            var projectDirectories = Directory
                .EnumerateDirectories(CorpusDirectory);

            var projects = projectDirectories
                .Select(dir => new Project(dir))
                .ToList();

            Console.WriteLine("Searching in: " + CorpusDirectory);
            Console.WriteLine("Projects: " + projects.Count);
            Console.WriteLine("Files: " + projects.Sum(p => p.FileCount));
            Console.WriteLine("Classes: " + projects.Sum(p => p.ClassCount));
            Console.WriteLine("Extending Classes: " + projects.Sum(p => p.ExtendingClassCount));
            Console.WriteLine("Methods: " + projects.Sum(p => p.MethodCount));
            Console.WriteLine("Virtual Methods: " + projects.Sum(p => p.VirtualMethodCount));
            Console.WriteLine("Override Methods: " + projects.Sum(p => p.OverrideMethodCount));

            Console.ReadLine();
        }
    }
}
