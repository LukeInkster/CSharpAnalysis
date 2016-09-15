using System;
using System.Linq;

namespace CSharpAnalysis
{
    public class Program
    {
        //private const string CorpusDirectory = @"C:\Dev\test\";
        //private const string CorpusDirectory = @"C:\Dev\CSharpCorpus\mono\mcs";
        private const string CorpusDirectory = @"C:\Dev\CSharpCorpusMini\";
        //private const string CorpusDirectory = @"C:\Dev\CSharpCorpusDotnet\corefx\src";
        //private const string CorpusDirectory = @"C:\Dev\CSharpCorpusLargerSeven\";
        //private const string CorpusDirectory = @"C:\Dev\CSharpCorpusMono\mono\";
        //private const string CorpusDirectory = @"C:\Dev\CSharpCorpusPowershell\";

        public static void Main(string[] args)
        {
            var corpus = new Corpus(CorpusDirectory);

            Console.WriteLine("Searching in: " + CorpusDirectory);
            Console.WriteLine("Projects: " + corpus.ProjectCount);
            Console.WriteLine("Files: " + corpus.FileCount);
            Console.WriteLine("Classes: " + corpus.ClassCount);
            Console.WriteLine("Extending Classes: " + corpus.ExtendingClassCount);
            Console.WriteLine("Extended Classes: " + corpus.ExtendedClassCount);
            Console.WriteLine("Methods: " + corpus.MethodCount);
            Console.WriteLine("Virtual Methods: " + corpus.VirtualMethodCount);
            Console.WriteLine("Override Methods: " + corpus.OverrideMethodCount);
            Console.WriteLine("Delegates: " + corpus.DelegateCount);
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

            foreach (var subclassCount in Project.SubclassCountToFrequency.Select(x => x).OrderBy(x => x.Key))
            {
                Console.WriteLine(subclassCount.Key  + "  :  " + subclassCount.Value);
            }

            Console.ReadLine();
        }
    }
}
