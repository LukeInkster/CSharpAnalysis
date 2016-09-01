using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public static class ParseTreeExtensions
    {
        public static IEnumerable<IParseTree> Children(this IParseTree tree)
        {
            return Enumerable
                .Range(0, tree.ChildCount)
                .Select(tree.GetChild);
        }

        public static IEnumerable<IParseTree> GrandChildren(this IParseTree tree)
        {
            return tree
                .Children()
                .SelectMany(child => child.Children());
        }

        public static IEnumerable<IParseTree> ChildrenWhere(this IParseTree tree, Func<IParseTree, bool> predicate)
        {
            return tree
                .Children()
                .Where(predicate);
        }

        public static IEnumerable<IParseTree> GrandChildrenWhere(this IParseTree tree, Func<IParseTree, bool> predicate)
        {
            return tree
                .GrandChildren()
                .Where(predicate);
        }

        public static bool AnyChild(this IParseTree tree, Func<IParseTree, bool> predicate)
        {
            return tree
                .Children()
                .Any(predicate);
        }

        public static bool AnyGrandChild(this IParseTree tree, Func<IParseTree, bool> prediate)
        {
            return tree
                .GrandChildren()
                .Any(prediate);
        }

        private static IEnumerable<IParseTree> Modifiers(this CSharpParser.Class_member_declarationContext context)
        {
            return context
                .Children()
                .OfType<CSharpParser.All_member_modifiersContext>()
                .SelectMany(c => c.Children());
        }

        public static bool TextEqualsIgnoreCase(this IParseTree context, string text)
        {
            return context
                .GetText()
                .Equals(text, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsMethodDeclaration(this IParseTree context)
        {
            // Void methods:
            // -  a grandchild node is a Method_declarationContext
            // Non-void methods:
            // -  a grandchild node is Typed_member_declarationContext which has a Method_declarationContext child
            return context.AnyGrandChild(grandChild =>
                    grandChild.IsMethodDeclarationTree() ||
                        (
                            grandChild is CSharpParser.Typed_member_declarationContext &&
                            grandChild.AnyChild(child => child.IsMethodDeclarationTree())
                        )
                    );
        }

        private static bool IsMethodDeclarationTree(this IParseTree context)
        {
            return context is CSharpParser.Method_declarationContext;
        }

        public static bool IsVirtualMethodDeclaration(this IParseTree context)
        {
            return (context as CSharpParser.Class_member_declarationContext)
                ?.AnyModifier(IsVirtualModifier)
                ?? false;
        }

        public static bool IsOverrideMethodDeclaration(this IParseTree context)
        {
            return (context as CSharpParser.Class_member_declarationContext)
                ?.AnyModifier(IsOverrideModifier)
                ?? false;
        }

        public static bool IsAbstractMethodDeclaration(this IParseTree context)
        {
            return (context as CSharpParser.Class_member_declarationContext)
                ?.AnyModifier(IsAbstractModifier)
                ?? false;
        }

        public static bool AnyModifier(this CSharpParser.Class_member_declarationContext tree, Func<IParseTree, bool> predicate)
        {
            return tree
                .Modifiers()
                .Any(predicate);
        }

        public static IEnumerable<T> ChildrenOfType<T>(this IParseTree context) where T : IParseTree
        {
            return context
                .Children()
                .OfType<T>();
        }

        public static IEnumerable<T> GrandChildrenOfType<T>(this IParseTree context) where T : IParseTree
        {
            return context
                .GrandChildren()
                .OfType<T>();
        }

        private static bool IsVirtualModifier(IParseTree tree)
        {
            return tree.TextEqualsIgnoreCase("virtual");
        }

        private static bool IsOverrideModifier(IParseTree tree)
        {
            return tree.TextEqualsIgnoreCase("override");
        }

        private static bool IsAbstractModifier(IParseTree tree)
        {
            return tree.TextEqualsIgnoreCase("abstract");
        }
    }
}
