using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;

namespace CSharpAnalysis
{
    public class FileAnalysis
    {
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }

        public FileAnalysis(string file)
        {
            using (var fileStream = new StreamReader(file))
            {
                var inputStream = new AntlrInputStream(fileStream);

                var visitor = new CSharpVisitor();

                visitor.VisitCompilation_unit(CompilationUnitFor(inputStream));

                ClassCount = visitor.ClassCount;
                MethodCount = visitor.MethodCount;
                VirtualMethodCount = visitor.VirtualMethodCount;
                OverrideMethodCount = visitor.OverrideMethodCount;
            }
        }

        private CSharpParser.Compilation_unitContext CompilationUnitFor(ICharStream inputStream)
        {
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
    }
}
