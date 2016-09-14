using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class Project
    {
        public readonly List<ClassAnalysis> ClassAnalyses;

        public int FileCount { get; private set; }
        public int ClassCount => ClassAnalyses.Count();

        public Project(string projectDirectory)
        {
            var cSharpFiles = CSharpFilesIn(projectDirectory);
            FileCount = cSharpFiles.Count;
            var classDefinitions = cSharpFiles.SelectMany(ClassDefinitions);
            ClassAnalyses = classDefinitions.Select(c => new ClassAnalysis(c)).ToList();

            var classNameToAnalysis = new Dictionary<string, ClassAnalysis>();
            var classNameToSubclasses = new Dictionary<string, List<ClassAnalysis>>();

            foreach (var classAnalysis in ClassAnalyses)
            {
                // Do a first pass to fill in class/superclass names and method details
                classAnalysis.FirstPassDetails();
                // Then put it in a map so other analyses can find it
                classNameToAnalysis[classAnalysis.ClassName] = classAnalysis;

                if (classAnalysis.SuperClassName != null)
                {
                    if (!classNameToSubclasses.ContainsKey(classAnalysis.SuperClassName))
                    {
                        classNameToSubclasses[classAnalysis.SuperClassName] = new List<ClassAnalysis>();
                    }
                    classNameToSubclasses[classAnalysis.SuperClassName].Add(classAnalysis);
                }
            }

            foreach (var classAnalysis in ClassAnalyses)
            {
                classAnalysis.Analyse(classNameToAnalysis, classNameToSubclasses);
            }
        }

        private static IEnumerable<CSharpParser.Class_definitionContext> ClassDefinitions(string file)
        {
            using (var fileStream = new StreamReader(file))
            {
                return ClassDefinitionsIn(CompilationUnitOf(fileStream));
            }
        }

        private static List<string> CSharpFilesIn(string dir)
        {
            return Directory.EnumerateFiles(
                path: dir,
                searchPattern: "*.cs",
                searchOption: SearchOption.AllDirectories)
                .ToList();
        }

        private static List<CSharpParser.Class_definitionContext> ClassDefinitionsIn(IParseTree tree)
        {
            var children = tree.Children().ToList();

            return children
                .OfType<CSharpParser.Class_definitionContext>()
                .Concat(children.SelectMany(ClassDefinitionsIn))
                .ToList();
        }

        private static CSharpParser.Compilation_unitContext CompilationUnitOf(TextReader streamReader)
        {
            var inputStream = new AntlrInputStream(streamReader);

            var codeTokens = new List<IToken>();
            var commentTokens = new List<IToken>();

            var preprocessorLexer = new CSharpLexer(inputStream);
            preprocessorLexer.RemoveErrorListeners();
            // Collect all tokens with lexer (CSharpLexer.g4).
            var tokens = preprocessorLexer.GetAllTokens();
            var directiveTokens = new List<IToken>();
            var directiveTokenSource = new ListTokenSource(directiveTokens);
            var directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
            var preprocessorParser = new CSharpPreprocessorParser(directiveTokenStream);
            preprocessorParser.RemoveErrorListeners();
            var index = 0;
            var compiliedTokens = true;
            while (index < tokens.Count)
            {
                var token = tokens[index];
                if (token.Type == CSharpLexer.SHARP)
                {
                    directiveTokens.Clear();
                    var directiveTokenIndex = index + 1;
                    // Collect all preprocessor directive tokens.
                    while (directiveTokenIndex < tokens.Count &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.Eof &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.DIRECTIVE_NEW_LINE &&
                           tokens[directiveTokenIndex].Type != CSharpLexer.SHARP)
                    {
                        if (tokens[directiveTokenIndex].Channel == CSharpLexer.COMMENTS_CHANNEL)
                        {
                            commentTokens.Add(tokens[directiveTokenIndex]);
                        }
                        else if (tokens[directiveTokenIndex].Channel != Lexer.Hidden)
                        {
                            directiveTokens.Add(tokens[directiveTokenIndex]);
                        }
                        directiveTokenIndex++;
                    }

                    directiveTokenSource = new ListTokenSource(directiveTokens);
                    directiveTokenStream = new CommonTokenStream(directiveTokenSource, CSharpLexer.DIRECTIVE);
                    preprocessorParser.SetInputStream(directiveTokenStream);
                    preprocessorParser.Reset();
                    // Parse condition in preprocessor directive (based on CSharpPreprocessorParser.g4 grammar).
                    var directive = preprocessorParser.preprocessor_directive();
                    // if true than next code is valid and not ignored.
                    compiliedTokens = directive.value;
                    index = directiveTokenIndex - 1;
                }
                else if (token.Channel != Lexer.Hidden && token.Type != CSharpLexer.DIRECTIVE_NEW_LINE && compiliedTokens)
                {
                    codeTokens.Add(token); // Collect code tokens.
                }
                index++;
            }

            // At second stage tokens parsed in usual way.
            var codeTokenSource = new ListTokenSource(tokens);
            var codeTokenStream = new CommonTokenStream(codeTokenSource);
            var parser = new CSharpParser(codeTokenStream);
            parser.RemoveErrorListeners();
            // Parse syntax tree (CSharpParser.g4)
            return parser.compilation_unit();
        }

        public static void PrintToOutput(string s)
        {
            System.Diagnostics.Debug.Write("\n" + s);
        }
    }
}
