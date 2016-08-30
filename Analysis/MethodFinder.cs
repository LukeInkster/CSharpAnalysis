
using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class MethodFinder : CSharpParserBaseVisitor<int>
    {
        public Dictionary<string, MethodDetails> AllMethodDetails = new Dictionary<string, MethodDetails>();

        public MethodDetails this[string key] => AllMethodDetails.ContainsKey(key) ? AllMethodDetails[key] : null;

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (!MemberDeclarationIsMethod(context)) return base.VisitClass_member_declaration(context);

            var methodDetails = new MethodDetails
            {
                IsVirtual = MethodDeclarationIsVirtual(context),
                IsOverride = MethodDeclarationIsOverride(context)
            };

            AllMethodDetails[MethodName(context)] = methodDetails;

            return base.VisitClass_member_declaration(context);
        }

        private static string MethodName(CSharpParser.Class_member_declarationContext context)
        {
            var grandChildren = GrandChildrenOf(context).ToList();

            var methodDeclaration = grandChildren
                .Where(IsMethodDeclarationTree)
                .Concat(grandChildren
                    .Where(grandChild => grandChild is CSharpParser.Typed_member_declarationContext)
                    .SelectMany(grandChild => ChildrenOf(grandChild).Where(IsMethodDeclarationTree))
                )
                .FirstOrDefault() as CSharpParser.Method_declarationContext;

            if (methodDeclaration == null) return string.Empty;

            var methodMemberNameContext = ChildrenOf(methodDeclaration)
                .OfType<CSharpParser.Method_member_nameContext>()
                .FirstOrDefault();

            return methodMemberNameContext == null ? string.Empty : methodMemberNameContext.GetText();
        }

        private static bool MethodDeclarationIsVirtual(CSharpParser.Class_member_declarationContext context)
        {
            return ModifiersOf(context)
                .Any(IsVirtualModifier);
        }

        private static bool MethodDeclarationIsOverride(CSharpParser.Class_member_declarationContext context)
        {
            return ModifiersOf(context)
                .Any(IsOverrideModifier);
        }

        private static IEnumerable<IParseTree> ModifiersOf(CSharpParser.Class_member_declarationContext context)
        {
            return context
                .children
                .OfType<CSharpParser.All_member_modifiersContext>()
                .SelectMany(ChildrenOf);
        }

        private static bool MemberDeclarationIsMethod(CSharpParser.Class_member_declarationContext context)
        {
            // Void methods:
            // -  a grandchild node is a Method_declarationContext
            // Non-void methods:
            // -  a grandchild node is Typed_member_declarationContext which has a Method_declarationContext child
            return GrandChildrenOf(context)
                .Any(grandChild =>
                    IsMethodDeclarationTree(grandChild) ||
                        (
                            grandChild is CSharpParser.Typed_member_declarationContext &&
                            ChildrenOf(grandChild).Any(IsMethodDeclarationTree)
                        )
                    );
        }

        private static bool IsMethodDeclarationTree(IParseTree tree)
        {
            return tree is CSharpParser.Method_declarationContext;
        }

        private static IEnumerable<IParseTree> ChildrenOf(IParseTree tree)
        {
            return Enumerable
                .Range(0, tree.ChildCount)
                .Select(tree.GetChild);
        }

        private static IEnumerable<IParseTree> GrandChildrenOf(IParseTree tree)
        {
            return ChildrenOf(tree)
                .SelectMany(ChildrenOf);
        }

        private static bool IsVirtualModifier(IParseTree tree)
        {
            return tree
                .GetText()
                .Equals("virtual", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOverrideModifier(IParseTree tree)
        {
            return tree
                .GetText()
                .Equals("override", StringComparison.OrdinalIgnoreCase);
        }

    }

    public class MethodDetails
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
    }
}
