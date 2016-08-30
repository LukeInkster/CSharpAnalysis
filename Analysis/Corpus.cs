using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Corpus
    {
        public readonly List<Project> Projects;

        public int ProjectCount => Projects.Count;
        public int FileCount => Projects.Sum(p => p.FileCount);
        public int ClassCount => Projects.Sum(p => p.ClassCount);
        public int ExtendingClassCount => Projects.Sum(p => p.ExtendingClassCount);
        public int MethodCount => Projects.Sum(p => p.MethodCount);
        public int VirtualMethodCount => Projects.Sum(p => p.VirtualMethodCount);
        public int OverrideMethodCount => Projects.Sum(p => p.OverrideMethodCount);
        public int ClassesWithVirtualDowncallsInConstructorsCount
            => AllClassAnalyses().Count(c => c.ContainsVirtualDowncallInConstructor);
        public int ClassesWithOverrideDowncallsInConstructorsCount
            => AllClassAnalyses().Count(c => c.ContainsOverrideDowncallInConstructor);

        public IEnumerable<ClassAnalysis> AllClassAnalyses()
        {
            return Projects
                .SelectMany(p => p.Analyses)
                .SelectMany(a => a.ClassAnalyses);
        }

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
