using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class CSharpVisitor : CSharpParserBaseVisitor<int>
    {
        public int MethodCount = 0;
        public int VirtualMethodCount = 0;

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (DeclarationIsMethod(context))
            {
                MethodCount++;
                if (MethodDeclarationIsVirtual(context))
                {
                    VirtualMethodCount++;
                }
            }
            return base.VisitClass_member_declaration(context);
        }

        private static bool MethodDeclarationIsVirtual(CSharpParser.Class_member_declarationContext context)
        {
            var result = context
                .children
                .OfType<CSharpParser.All_member_modifiersContext>()
                .SelectMany(ChildrenOf)
                .Any(IsVirtualModifier);
            
            if (result == false && context.GetText().Contains("virtual"))
            {
                Print("hi");
            }

            return result;
        }

        private static bool DeclarationIsMethod(CSharpParser.Class_member_declarationContext context)
        {
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

        private static bool IsVirtualModifier(IParseTree child)
        {
            return child
                .GetText()
                .ToLower()
                .Equals("virtual");
        }

        public static void Print(string s)
        {
            System.Diagnostics.Debug.Write("\n"+s);
        }
    }
}
