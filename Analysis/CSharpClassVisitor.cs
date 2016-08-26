using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class CSharpClassVisitor : CSharpParserBaseVisitor<int>
    {
        public bool ClassIsExtending { get; private set; }
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }

        private bool _alreadyInClass;

        public override int VisitClass_definition(CSharpParser.Class_definitionContext context)
        {
            if (_alreadyInClass)
            {
                // each class has its own visitor, so if we find one when we're already in
                // a class then we leave it to the other visitor that will be created
                return 0;
            }
            _alreadyInClass = true;
            if (ClassHasSuperClass(context))
            {
                ClassIsExtending = true;
            }
            return base.VisitClass_definition(context);
        }

        private static bool ClassHasSuperClass(CSharpParser.Class_definitionContext context)
        {
            return context
                .children
                .Any(child => child is CSharpParser.Class_baseContext);
        }

        public override int VisitClass_body(CSharpParser.Class_bodyContext context)
        {
            ClassCount++;
            return base.VisitClass_body(context);
        }

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (MemberDeclarationIsMethod(context))
            {
                MethodCount++;
                if (MethodDeclarationIsVirtual(context))
                {
                    VirtualMethodCount++;
                }
                if (MethodDeclarationIsOverride(context))
                {
                    OverrideMethodCount++;
                }
            }
            return base.VisitClass_member_declaration(context);
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

        public static void Print(string s)
        {
            System.Diagnostics.Debug.Write("\n"+s);
        }
    }
}
