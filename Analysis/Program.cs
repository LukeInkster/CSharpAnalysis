using System;

namespace CSharpAnalysis
{
    public class Program
    {
        //private const string CorpusDirectory = @"C:\Dev\test\";
        private const string CorpusDirectory = @"C:\Dev\CSharpCorpusMini\";

        public static void Main(string[] args)
        {
            var corpus = new Corpus(CorpusDirectory);

            Console.WriteLine("Searching in: " + CorpusDirectory);
            Console.WriteLine("Projects: " + corpus.ProjectCount);
            Console.WriteLine("Files: " + corpus.FileCount);
            Console.WriteLine("Classes: " + corpus.ClassCount);
            Console.WriteLine("Extending Classes: " + corpus.ExtendingClassCount);
            Console.WriteLine("Methods: " + corpus.MethodCount);
            Console.WriteLine("Virtual Methods: " + corpus.VirtualMethodCount);
            Console.WriteLine("Override Methods: " + corpus.OverrideMethodCount);
            Console.WriteLine("Classes with virtual downcalls in constructors: "
                + corpus.ClassesWithVirtualDowncallsInConstructorsCount);

            Console.ReadLine();
        }
    }
}
