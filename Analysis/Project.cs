using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Project
    {
        public readonly List<FileAnalysis> Analyses;

        public int FileCount => Analyses.Count;
        public int ClassCount => Analyses.Sum(a => a.ClassCount);
        public int ExtendingClassCount => Analyses.Sum(a => a.ExtendingClassCount);
        public int MethodCount => Analyses.Sum(a => a.MethodCount);
        public int VirtualMethodCount => Analyses.Sum(a => a.VirtualMethodCount);
        public int OverrideMethodCount => Analyses.Sum(a => a.OverrideMethodCount);

        public Project(string projectDirectory)
        {
            Analyses = Analyse(CSharpFilesIn(projectDirectory));
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
