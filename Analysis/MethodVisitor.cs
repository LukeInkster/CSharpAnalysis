using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public class MethodVisitor : CSharpParserBaseVisitor<int>
    {
        public FirstPassDetails FirstPassDetails = new FirstPassDetails();

        public MethodDetails this[string key] => 
            FirstPassDetails.AllMethodDetails.ContainsKey(key)
            ? FirstPassDetails.AllMethodDetails[key]
            : null;

        private bool _alreadyInClass;

        public override int VisitClass_definition(CSharpParser.Class_definitionContext context)
        {
            if (_alreadyInClass) return 0;

            Console.WriteLine("test");
            _alreadyInClass = true;
            FirstPassDetails.ClassName = ClassName(context);
            FirstPassDetails.SuperClassName = SuperClassName(context);

            return base.VisitClass_definition(context);
        }

        private static string ClassName(CSharpParser.Class_definitionContext context)
        {
            return Format(context
                .ChildrenOfType<CSharpParser.IdentifierContext>()
                .FirstOrDefault()
                ?.GetText());
        }

        private static string SuperClassName(CSharpParser.Class_definitionContext context)
        {
            var superClassName = Format(context
                .ChildrenOfType<CSharpParser.Class_baseContext>()
                .FirstOrDefault()
                ?.ChildrenOfType<CSharpParser.Class_typeContext>()
                ?.FirstOrDefault()
                ?.GetText());

            if (superClassName == null || superClassName.Length < 2 ||
                superClassName[0] == 'I' && char.IsUpper(superClassName[1]))
            {
                return null;
            }

            return superClassName;
        }

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (!context.IsMethodDeclaration()) return base.VisitClass_member_declaration(context);

            var methodDetails = new MethodDetails
            {
                IsVirtual = context.IsVirtualMethodDeclaration(),
                IsOverride = context.IsOverrideMethodDeclaration(),
                IsAbstract = context.IsAbstractMethodDeclaration()
            };

            FirstPassDetails.AllMethodDetails[MethodName(context)] = methodDetails;

            return base.VisitClass_member_declaration(context);
        }

        public override int VisitStruct_member_declaration(CSharpParser.Struct_member_declarationContext context)
        {
            if (!context.IsMethodDeclaration()) return base.VisitStruct_member_declaration(context);

            var methodDetails = new MethodDetails
            {
                IsVirtual = context.IsVirtualMethodDeclaration(),
                IsOverride = context.IsOverrideMethodDeclaration(),
                IsAbstract = context.IsAbstractMethodDeclaration()
            };

            FirstPassDetails.AllMethodDetails[MethodName(context)] = methodDetails;

            return base.VisitStruct_member_declaration(context);
        }

        public override int VisitDelegate_definition(CSharpParser.Delegate_definitionContext context)
        {
            var methodDetails = new MethodDetails
            {
                IsDelegate = true
            };

            FirstPassDetails.AllMethodDetails[DelegateName(context)] = methodDetails;
            return base.VisitDelegate_definition(context);
        }

        private static string DelegateName(CSharpParser.Delegate_definitionContext context)
        {
            return context
                .ChildrenOfType<CSharpParser.IdentifierContext>()
                .FirstOrDefault()
                ?.GetText()
                ?? string.Empty;
        }

        private static string MethodName(IParseTree context)
        {
            // Void methods:
            // -  a grandchild node is a Method_declarationContext
            // Non-void methods:
            // -  a grandchild node is Typed_member_declarationContext which has a Method_declarationContext child
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

            return methodMemberNameContext == null
                ? string.Empty
                : Format(methodMemberNameContext.GetText());
        }

        private static string Format(string name)
        {
            return name;//?.Split('<')[0];
        }
    }

    public class FirstPassDetails
    {
        public string ClassName { get; set; }
        public string SuperClassName { get; set; }
        public Dictionary<string, MethodDetails> AllMethodDetails { get; set; }

        public FirstPassDetails()
        {
            AllMethodDetails = new Dictionary<string, MethodDetails>();
        }
    }

    public class MethodDetails
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsDelegate { get; set; }
    }
}
