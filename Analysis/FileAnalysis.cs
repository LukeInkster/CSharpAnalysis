using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class FileAnalysis
    {
        private readonly IEnumerable<ClassAnalysis> _classAnalyses;
        public int ClassCount => _classAnalyses.Sum(c => c.ClassCount);
        public int ExtendingClassCount => _classAnalyses.Sum(c => c.ExtendingClassCount);
        public int MethodCount => _classAnalyses.Sum(c => c.MethodCount);
        public int VirtualMethodCount => _classAnalyses.Sum(c => c.VirtualMethodCount);
        public int OverrideMethodCount => _classAnalyses.Sum(c => c.OverrideMethodCount);

        public FileAnalysis(string file)
        {
            using (var fileStream = new StreamReader(file))
            {
                _classAnalyses = Analyse(ClassDefinitionsIn(CompilationUnitOf(fileStream)));
            }
        }

        private static IEnumerable<ClassAnalysis> Analyse(IEnumerable<CSharpParser.Class_definitionContext> classDefinitions)
        {
            return classDefinitions
                .Select(classDef => new ClassAnalysis(classDef));
        }

        private static IEnumerable<CSharpParser.Class_definitionContext> ClassDefinitionsIn(IParseTree tree)
        {
            var children = ChildrenOf(tree).ToList();

            return children
                .OfType<CSharpParser.Class_definitionContext>()
                .Concat(children.SelectMany(ClassDefinitionsIn));
        }

        private static CSharpParser.Compilation_unitContext CompilationUnitOf(TextReader streamReader)
        {
            var inputStream = new AntlrInputStream(streamReader);

            var codeTokens = new List<IToken>();
            var commentTokens = new List<IToken>();

            var preprocessorLexer = new CSharpLexer(inputStream);
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
            // Parse syntax tree (CSharpParser.g4)
            return parser.compilation_unit();
        }

        private static IEnumerable<IParseTree> ChildrenOf(IParseTree tree)
        {
            return Enumerable
                .Range(0, tree.ChildCount)
                .Select(tree.GetChild);
        }
    }
}
