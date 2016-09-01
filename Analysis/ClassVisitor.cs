using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalysis
{
    public class ClassVisitor : CSharpParserBaseVisitor<int>
    {
        public string SuperClassName => _firstPassDetails.ClassName;
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }
        public bool ContainsLocalVirtualCallInConstructor { get; private set; }
        public bool ContainsLocalOverrideCallInConstructor { get; private set; }
        public bool ContainsLocalAbstractCallInConstructor { get; private set; }
        public bool ContainsLocalMethodCallInConstructor { get; private set; }
        public bool ContainsUntracedMethodCallInConstructor { get; private set; }

        private bool _alreadyInClass;
        private bool _inConstructor;
        private readonly FirstPassDetails _firstPassDetails;
        private readonly Dictionary<string, ClassAnalysis> _classNameToAnalysis;

        public ClassVisitor(FirstPassDetails firstPassDetails, Dictionary<string, ClassAnalysis> classNameToAnalysis)
        {
            _firstPassDetails = firstPassDetails;
            _classNameToAnalysis = classNameToAnalysis;
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
                ContainsLocalMethodCallInConstructor = true;

                var methodName = GetLocalMethodCallName(context);
                MethodDetails methodDetails = null;
                var classToCheck = _firstPassDetails;

                // Try to find a class in the hierarchy that defines the method we're looking for
                while (methodDetails == null && classToCheck != null)
                {
                    methodDetails = classToCheck.AllMethodDetails.ContainsKey(methodName)
                        ? classToCheck.AllMethodDetails[methodName]
                        : null;

                    if (classToCheck.SuperClassName == null ||
                        !_classNameToAnalysis.ContainsKey(classToCheck.SuperClassName)) break;

                    classToCheck = _classNameToAnalysis[classToCheck.SuperClassName].FirstPassDetails();
                }

                // We couldn't find the method
                if (methodDetails == null)
                {
                    ContainsUntracedMethodCallInConstructor = true;
                }
                // We found the method, look at its modifiers
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
                    if (methodDetails.IsAbstract)
                    {
                        ContainsLocalAbstractCallInConstructor = true;
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

            return base.VisitClass_definition(context);
        }

        public override int VisitClass_body(CSharpParser.Class_bodyContext context)
        {
            ClassCount++;
            return base.VisitClass_body(context);
        }

        public override int VisitClass_member_declaration(CSharpParser.Class_member_declarationContext context)
        {
            if (context.IsMethodDeclaration())
            {
                MethodCount++;
                if (context.IsVirtualMethodDeclaration())
                {
                    VirtualMethodCount++;
                }
                if (context.IsOverrideMethodDeclaration())
                {
                    OverrideMethodCount++;
                }
            }
            return base.VisitClass_member_declaration(context);
        }

        public static void Print(string s)
        {
            System.Diagnostics.Debug.Write("\n"+s);
        }
    }
}
