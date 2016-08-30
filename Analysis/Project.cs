using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpAnalysis
{
    public class Project
    {
        public readonly List<FileAnalysis> FileAnalyses;

        public int FileCount => FileAnalyses.Count;

        public Project(string projectDirectory)
        {
            FileAnalyses = Analyse(CSharpFilesIn(projectDirectory));
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
