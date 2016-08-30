using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Corpus
    {
        public readonly List<Project> Projects;
        public IEnumerable<ClassAnalysis> AllClassAnalyses => Projects
                .SelectMany(p => p.FileAnalyses)
                .SelectMany(a => a.ClassAnalyses);

        public int ProjectCount => Projects.Count;
        public int FileCount => Projects.Sum(p => p.FileCount);
        public int ClassCount => AllClassAnalyses.Count();
        public int ExtendingClassCount => AllClassAnalyses.Count(p => p.ClassIsExtending);
        public int MethodCount => AllClassAnalyses.Sum(p => p.MethodCount);
        public int VirtualMethodCount => AllClassAnalyses.Sum(c => c.VirtualMethodCount);
        public int OverrideMethodCount => AllClassAnalyses.Sum(c => c.OverrideMethodCount);
        public int ClassesWithVirtualDowncallsInConstructorsCount
            => AllClassAnalyses.Count(c => c.ContainsLocalVirtualCallInConstructor);
        public int ClassesWithOverrideDowncallsInConstructorsCount
            => AllClassAnalyses.Count(c => c.ContainsLocalOverrideCallInConstructor);
        public int ClassesWithLocalMethodCallsInConstructosCount
            => AllClassAnalyses.Count(c => c.ContainsLocalMethodCallInConstructor);
        public int ClassesWithUntracedCallsInConstructorsCount
            => AllClassAnalyses.Count(c => c.ContainsUntracedMethodCallInConstructor);

        public Corpus(string directory)
        {
            var projectDirectories = Directory
                .EnumerateDirectories(directory);

            Projects = projectDirectories
                .Select(dir => new Project(dir))
                .ToList();
        }
    }
}
