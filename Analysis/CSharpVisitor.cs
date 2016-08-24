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

        public override int VisitMethod_body(CSharpParser.Method_bodyContext context)
        {
            Print("hi");
            return base.VisitMethod_body(context);
        }
        public override int VisitMethod_declaration(CSharpParser.Method_declarationContext context)
        {
            //Print(context.GetText());
            return base.VisitMethod_declaration(context);
        }

        public override int VisitMethod_member_name(CSharpParser.Method_member_nameContext context)
        {
            //Print(context.GetText());
            //Print(context.parent.parent.parent.GetType().ToString());
            return base.VisitMethod_member_name(context);
        }

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
            //foreach (var parseTree in context.children)
            //{
            //    Print("test");
            //    Print(parseTree.GetType().FullName);
            //    Print(parseTree.GetText());
            //}
            Print(context.GetText());
            return base.VisitClass_member_declaration(context);
        }

        private static bool MethodDeclarationIsVirtual(CSharpParser.Class_member_declarationContext context)
        {
            return ChildrenOf(context.children[0])
                .Any(IsVirtualModifier);
        }

        private static bool DeclarationIsMethod(CSharpParser.Class_member_declarationContext context)
        {
            Print(context.GetText());

            var grandChildren = GrandChildrenOf(context);

            return GrandChildrenOf(context)
                .Any(grandChild =>
                    IsMethodDeclarationNode(grandChild) ||
                        (
                            grandChild is CSharpParser.Typed_member_declarationContext &&
                            ChildrenOf(grandChild).Any(IsMethodDeclarationNode)
                        )
                    );
        }

        private static bool IsMethodDeclarationNode(IParseTree grandChild)
        {
            return grandChild is CSharpParser.Method_declarationContext;
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

        public override int VisitAll_member_modifier(CSharpParser.All_member_modifierContext context)
        {
            //Print(context.GetText());
            //Print(context.parent.GetType().FullName);
            //Print(context.GetText());
            return base.VisitAll_member_modifier(context);
        }

        private static bool IsVirtualModifier(IParseTree child)
        {
            return child
                .GetText()
                .ToLower()
                .Equals("virtual");
        }

        public override int VisitAll_member_modifiers(CSharpParser.All_member_modifiersContext context)
        {
            //Print(context.GetText());
            //Print("\t" + "\t" + "\t" + "\t" + context.parent.parent.parent.parent.GetType().FullName);
            //Print("\t" + "\t" + "\t" + context.parent.parent.parent.GetType().FullName);
            //Print("\t" + "\t" + context.parent.parent.GetType().FullName);
            //Print("\t" + context.parent.GetType().FullName);
            //Print(context.GetType().FullName);
            //Print(context.children.Any(IsVirtualModifier).ToString());
            return base.VisitAll_member_modifiers(context);
        }

        public override int VisitClass_member_declarations(CSharpParser.Class_member_declarationsContext context)
        {
            //foreach (var decl in context.class_member_declaration())
            //{
            //    Print("a " + decl.GetText());
            //}

            return base.VisitClass_member_declarations(context);
        }

        public static void Print(string s)
        {
            System.Diagnostics.Debug.Write("\n"+s);
        }
    }
}
