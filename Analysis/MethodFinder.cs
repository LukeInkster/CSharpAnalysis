
using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class MethodFinder : CSharpParserBaseVisitor<int>
    {
        public Dictionary<string, MethodDetails> AllMethodDetails = new Dictionary<string, MethodDetails>();

        public MethodDetails this[string key] => AllMethodDetails.ContainsKey(key) ? AllMethodDetails[key] : null;

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (!context.IsMethodDeclaration()) return base.VisitClass_member_declaration(context);

            var methodDetails = new MethodDetails
            {
                IsVirtual = context.IsVirtualMethodDeclaration(),
                IsOverride = context.IsOverrideMethodDeclaration()
            };

            AllMethodDetails[MethodName(context)] = methodDetails;

            return base.VisitClass_member_declaration(context);
        }

        public override int VisitStruct_member_declaration(CSharpParser.Struct_member_declarationContext context)
        {
            if (!context.IsMethodDeclaration()) return base.VisitStruct_member_declaration(context);

            var methodDetails = new MethodDetails
            {
                IsVirtual = context.IsVirtualMethodDeclaration(),
                IsOverride = context.IsOverrideMethodDeclaration()
            };

            AllMethodDetails[MethodName(context)] = methodDetails;

            return base.VisitStruct_member_declaration(context);
        }

        private static string MethodName(IParseTree context)
        {
            var methodDeclaration = context
                .GrandChildrenOfType<CSharpParser.Method_declarationContext>()
                .Concat(context
                    .GrandChildrenOfType<CSharpParser.Typed_member_declarationContext>()
                    .SelectMany(grandChild => grandChild.ChildrenOfType<CSharpParser.Method_declarationContext>())
                )
                .FirstOrDefault();

            if (methodDeclaration == null) return string.Empty;

            var methodMemberNameContext = methodDeclaration
                .ChildrenOfType<CSharpParser.Method_member_nameContext>()
                .FirstOrDefault();

            return methodMemberNameContext == null ? string.Empty : methodMemberNameContext.GetText();
        }
    }

    public class MethodDetails
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
    }
}
