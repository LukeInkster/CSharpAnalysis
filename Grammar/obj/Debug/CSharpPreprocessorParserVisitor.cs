//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\luke.inkster\Documents\Visual Studio 2015\Projects\CSharpAnalysis\Grammar\CSharpPreprocessorParser.g4 by ANTLR 4.5.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace ConsoleApplication1 {
 using System.Linq; 
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="CSharpPreprocessorParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.3")]
[System.CLSCompliant(false)]
public interface ICSharpPreprocessorParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorDiagnostic</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorDiagnostic([NotNull] CSharpPreprocessorParser.PreprocessorDiagnosticContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorRegion</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorRegion([NotNull] CSharpPreprocessorParser.PreprocessorRegionContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorDeclaration</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorDeclaration([NotNull] CSharpPreprocessorParser.PreprocessorDeclarationContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorConditional</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorConditional([NotNull] CSharpPreprocessorParser.PreprocessorConditionalContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorPragma</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorPragma([NotNull] CSharpPreprocessorParser.PreprocessorPragmaContext context);

	/// <summary>
	/// Visit a parse tree produced by the <c>preprocessorLine</c>
	/// labeled alternative in <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessorLine([NotNull] CSharpPreprocessorParser.PreprocessorLineContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="CSharpPreprocessorParser.preprocessor_directive"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessor_directive([NotNull] CSharpPreprocessorParser.Preprocessor_directiveContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="CSharpPreprocessorParser.directive_new_line_or_sharp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirective_new_line_or_sharp([NotNull] CSharpPreprocessorParser.Directive_new_line_or_sharpContext context);

	/// <summary>
	/// Visit a parse tree produced by <see cref="CSharpPreprocessorParser.preprocessor_expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPreprocessor_expression([NotNull] CSharpPreprocessorParser.Preprocessor_expressionContext context);
}
} // namespace ConsoleApplication1
