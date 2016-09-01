using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalysis
{
    public class ClassAnalysis
    {
        public string SuperClassName { get; private set; }
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }
        public bool ContainsLocalVirtualCallInConstructor { get; private set; }
        public bool ContainsLocalOverrideCallInConstructor { get; private set; }
        public bool ContainsLocalAbstractCallInConstructor { get; private set; }
        public bool ContainsLocalMethodCallInConstructor { get; private set; }
        public bool ContainsUntracedMethodCallInConstructor { get; private set; }

        private CSharpParser.Class_definitionContext _classDef;
        private string _lazyClassName;
        private Dictionary<string, MethodDetails> _lazyMethodDetails;

        public ClassAnalysis(CSharpParser.Class_definitionContext classDef)
        {
            _classDef = classDef;
        }

        public void Analyse(Dictionary<string, Dictionary<string, MethodDetails>> classNameToMethodDetails)
        {
            // Visitors need to stay local scoped. It appears they keep a reference to the
            // trees they visit, so if we keep a reference to the visitor then we also
            // keep a transitive reference to the tree which prevents garbage collection
            var visitor = new ClassVisitor(MethodDetails());

            visitor.VisitClass_definition(_classDef);

            ClassCount = visitor.ClassCount;
            SuperClassName = visitor.SuperClassName;
            MethodCount = visitor.MethodCount;
            VirtualMethodCount = visitor.VirtualMethodCount;
            OverrideMethodCount = visitor.OverrideMethodCount;
            ContainsLocalMethodCallInConstructor = visitor.ContainsLocalMethodCallInConstructor;
            ContainsLocalVirtualCallInConstructor = visitor.ContainsLocalVirtualCallInConstructor;
            ContainsLocalOverrideCallInConstructor = visitor.ContainsLocalOverrideCallInConstructor;
            ContainsLocalAbstractCallInConstructor = visitor.ContainsLocalAbstractCallInConstructor;
            ContainsUntracedMethodCallInConstructor = visitor.ContainsUntracedMethodCallInConstructor;

            _classDef = null;
        }

        public string ClassName()
        {
            if (_lazyClassName != null) return _lazyClassName;

            _lazyClassName = _classDef
                .ChildrenOfType<CSharpParser.IdentifierContext>()
                .FirstOrDefault()
                ?.GetText();

            return _lazyClassName;
        }

        public Dictionary<string, MethodDetails> MethodDetails()
        {
            if (_lazyMethodDetails != null) return _lazyMethodDetails;

            var methodFinder = new MethodVisitor();
            methodFinder.VisitClass_definition(_classDef);

            _lazyMethodDetails = methodFinder.AllMethodDetails;
            return _lazyMethodDetails;
        }
    }
}
