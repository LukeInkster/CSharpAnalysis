using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class CSharpClassVisitor : CSharpParserBaseVisitor<int>
    {
        public string SuperClassName { get; private set; }
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }
        public bool ContainsLocalVirtualCallInConstructor { get; private set; }
        public bool ContainsLocalOverrideCallInConstructor { get; private set; }
        public bool ContainsLocalMethodCallInConstructor { get; private set; }
        public bool ContainsUntracedMethodCallInConstructor { get; private set; }

        private bool _alreadyInClass;
        private bool _inConstructor;
        private readonly MethodFinder _methodFinder;

        public CSharpClassVisitor(MethodFinder methodFinder)
        {
            _methodFinder = methodFinder;
        }

        public override int VisitConstructor_declaration(CSharpParser.Constructor_declarationContext context)
        {
            _inConstructor = true;
            var result = base.VisitConstructor_declaration(context);
            _inConstructor = false;
            return result;
        }

        public override int VisitPrimary_expression(CSharpParser.Primary_expressionContext context)
        {
            if (_inConstructor && IsLocalMethodCall(context))
            {
                var methodName = GetLocalMethodCallName(context);
                var methodDetails = _methodFinder[methodName];

                ContainsLocalMethodCallInConstructor = true;
                if (methodDetails == null)
                {
                    ContainsUntracedMethodCallInConstructor = true;
                }
                else
                {
                    if (methodDetails.IsVirtual)
                    {
                        ContainsLocalVirtualCallInConstructor = true;
                    }
                    if (methodDetails.IsOverride)
                    {
                        ContainsLocalOverrideCallInConstructor = true;
                    }
                }
            }
            return base.VisitPrimary_expression(context);
        }

        private static string GetLocalMethodCallName(CSharpParser.Primary_expressionContext context)
        {
            var name = context
                .children
                .FirstOrDefault(child => child is CSharpParser.SimpleNameExpressionContext)
                ?.GetText();

            if (name != null)
            {
                return name;
            }

            var memberAccess = context
                .children
                .FirstOrDefault(child => child is CSharpParser.Member_accessContext)
                as CSharpParser.Member_accessContext;

            return memberAccess
                ?.children
                ?.FirstOrDefault(child => child is CSharpParser.IdentifierContext)
                ?.GetText()
                ?? string.Empty;
        }

        private static bool IsLocalMethodCall(CSharpParser.Primary_expressionContext context)
        {
            // method()
            if (context.ChildCount == 2 &&
                context.children[0] is CSharpParser.SimpleNameExpressionContext &&
                context.children[1] is CSharpParser.Method_invocationContext)
            {
                return true;
            }

            // this.method()
            return context.ChildCount == 3 &&
                   context.children[0].GetText() == "this" &&
                   context.children[1] is CSharpParser.Member_accessContext &&
                   context.children[2] is CSharpParser.Method_invocationContext;
        }

        public override int VisitClass_definition(CSharpParser.Class_definitionContext context)
        {
            if (_alreadyInClass)
            {
                // each class has its own visitor, so if we find one when we're already in
                // a class then we leave it to the other visitor that will be created
                return 0;
            }
            _alreadyInClass = true;
            SuperClassName = FindSuperClassName(context);

            return base.VisitClass_definition(context);
        }

        private static bool ClassHasSuperClass(CSharpParser.Class_definitionContext context)
        {
            return context
                .children
                .Any(child => child is CSharpParser.Class_baseContext);
        }

        private static string FindSuperClassName(CSharpParser.Class_definitionContext context)
        {
            return context
                .children
                .FirstOrDefault(child => child is CSharpParser.Class_baseContext)
                ?.Children()
                ?.FirstOrDefault(grandChild => grandChild is CSharpParser.Class_typeContext)
                ?.GetText();
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
