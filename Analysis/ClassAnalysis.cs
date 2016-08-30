namespace CSharpAnalysis
{
    public class ClassAnalysis
    {
        public bool ClassIsExtending { get; }
        public int ClassCount { get; }
        public int MethodCount { get; }
        public int VirtualMethodCount { get; }
        public int OverrideMethodCount { get; }
        public bool ContainsVirtualDowncallInConstructor { get; }
        public bool ContainsOverrideDowncallInConstructor { get; }

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
            ClassIsExtending = visitor.ClassIsExtending;
            MethodCount = visitor.MethodCount;
            VirtualMethodCount = visitor.VirtualMethodCount;
            OverrideMethodCount = visitor.OverrideMethodCount;
            ContainsVirtualDowncallInConstructor = visitor.ContainsVirtualDowncallInConstructor;
            ContainsOverrideDowncallInConstructor = visitor.ContainsOverrideDowncallInConstructor;
        }
    }
}
