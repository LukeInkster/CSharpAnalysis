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
            Console.WriteLine("Classes with calls to local methods in constructors: "
                + corpus.ClassesWithLocalMethodCallsInConstructorsCount);
            Console.WriteLine("Classes with calls to local virtual methods in constructors: "
                + corpus.ClassesWithVirtualDowncallsInConstructorsCount);
            Console.WriteLine("Classes with calls to local override methods in constructors: "
                + corpus.ClassesWithOverrideDowncallsInConstructorsCount);
            Console.WriteLine("Classes with calls to local abstract methods in constructors: "
                + corpus.ClassesWithAbstractDowncallsInConstructorsCount);
            Console.WriteLine("Classes with calls to methods that couldn't be found in constructors: "
                + corpus.ClassesWithUntracedCallsInConstructorsCount);

            Console.ReadLine();
        }
    }
}
