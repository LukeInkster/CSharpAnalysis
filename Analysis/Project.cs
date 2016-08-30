using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Project
    {
        private readonly List<FileAnalysis> _analyses;

        public int FileCount => _analyses.Count;
        public int ClassCount => _analyses.Sum(a => a.ClassCount);
        public int ExtendingClassCount => _analyses.Sum(a => a.ExtendingClassCount);
        public int MethodCount => _analyses.Sum(a => a.MethodCount);
        public int VirtualMethodCount => _analyses.Sum(a => a.VirtualMethodCount);
        public int OverrideMethodCount => _analyses.Sum(a => a.OverrideMethodCount);
        public int ClassesWithVirtualDowncallsInConstructorsCount
            => _analyses.Sum(a => a.ClassesWithVirtualDowncallsInConstructorsCount);

        public Project(string projectDirectory)
        {
            _analyses = Analyse(CSharpFilesIn(projectDirectory));
        }

        private static List<FileAnalysis> Analyse(List<string> files)
        {
            return files
                .Select(file => new FileAnalysis(file))
                .ToList();
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
