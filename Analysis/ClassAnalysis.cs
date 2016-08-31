namespace CSharpAnalysis
{
    public class ClassAnalysis
    {
        public string SuperClassName { get; }
        public int ClassCount { get; }
        public int MethodCount { get; }
        public int VirtualMethodCount { get; }
        public int OverrideMethodCount { get; }
        public bool ContainsLocalVirtualCallInConstructor { get; }
        public bool ContainsLocalOverrideCallInConstructor { get; }
        public bool ContainsLocalMethodCallInConstructor { get; }
        public bool ContainsUntracedMethodCallInConstructor { get; }

        public ClassAnalysis(CSharpParser.Class_definitionContext classDef)
        {
            var methodFinder = new MethodFinder();
            methodFinder.VisitClass_definition(classDef);

            // Visitors need to stay local scoped. It appears they keep a reference to the
            // trees they visit, so if we keep a reference to the visitor then we also
            // keep a transitive reference to the tree which prevents garbage collection
            var visitor = new CSharpClassVisitor(methodFinder);

            visitor.VisitClass_definition(classDef);

            ClassCount = visitor.ClassCount;
            SuperClassName = visitor.SuperClassName;
            MethodCount = visitor.MethodCount;
            VirtualMethodCount = visitor.VirtualMethodCount;
            OverrideMethodCount = visitor.OverrideMethodCount;
            ContainsLocalMethodCallInConstructor = visitor.ContainsLocalMethodCallInConstructor;
            ContainsLocalVirtualCallInConstructor = visitor.ContainsLocalVirtualCallInConstructor;
            ContainsLocalOverrideCallInConstructor = visitor.ContainsLocalOverrideCallInConstructor;
            ContainsUntracedMethodCallInConstructor = visitor.ContainsUntracedMethodCallInConstructor;
        }
    }
}
