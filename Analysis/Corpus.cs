using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Corpus
    {
        private readonly List<Project> _projects;

        public int ProjectCount => _projects.Count;
        public int FileCount => _projects.Sum(p => p.FileCount);
        public int ClassCount => _projects.Sum(p => p.ClassCount);
        public int ExtendingClassCount => _projects.Sum(p => p.ExtendingClassCount);
        public int MethodCount => _projects.Sum(p => p.MethodCount);
        public int VirtualMethodCount => _projects.Sum(p => p.VirtualMethodCount);
        public int OverrideMethodCount => _projects.Sum(p => p.OverrideMethodCount);

        public Corpus(string directory)
        {
            var projectDirectories = Directory
                .EnumerateDirectories(directory);

            _projects = projectDirectories
                .Select(dir => new Project(dir))
                .ToList();
        }
    }
}
