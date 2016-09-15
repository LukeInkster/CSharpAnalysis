using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalysis
{
    public class ClassAnalysis
    {
        public int ClassCount { get; private set; }
        public int MethodCount { get; private set; }
        public int VirtualMethodCount { get; private set; }
        public int OverrideMethodCount { get; private set; }
        public int DelegateCount { get; private set; }
        public bool ContainsLocalVirtualCallInConstructor { get; private set; }
        public bool ContainsLocalOverrideCallInConstructor { get; private set; }
        public bool ContainsLocalAbstractCallInConstructor { get; private set; }
        public bool ContainsLocalMethodCallInConstructor { get; private set; }
        public bool ContainsUntracedMethodCallInConstructor { get; private set; }
        public bool HasSubclasses { get; private set; }
        public string SuperClassName => FirstPassDetails().SuperClassName;
        public string ClassName => FirstPassDetails().ClassName;

        private CSharpParser.Class_definitionContext _classDef;
        private FirstPassDetails _lazyFirstPassDetails;

        public ClassAnalysis(CSharpParser.Class_definitionContext classDef)
        {
            _classDef = classDef;
        }

        public void Analyse(Dictionary<string, ClassAnalysis> classNameToAnalysis, Dictionary<string, List<ClassAnalysis>> classNameToSubclasses)
        {
            // Visitors need to stay local scoped. It appears they keep a reference to the
            // trees they visit, so if we keep a reference to the visitor then we also
            // keep a transitive reference to the tree which prevents garbage collection
            var visitor = new ClassVisitor(FirstPassDetails(), classNameToAnalysis);

            visitor.VisitClass_definition(_classDef);

            ClassCount = visitor.ClassCount;
            MethodCount = visitor.MethodCount;
            VirtualMethodCount = visitor.VirtualMethodCount;
            OverrideMethodCount = visitor.OverrideMethodCount;
            DelegateCount = FirstPassDetails().AllMethodDetails.Values.Count(method => method.IsDelegate);
            ContainsLocalMethodCallInConstructor = visitor.ContainsLocalMethodCallInConstructor;
            ContainsLocalVirtualCallInConstructor = visitor.ContainsLocalVirtualCallInConstructor;
            ContainsLocalOverrideCallInConstructor = visitor.ContainsLocalOverrideCallInConstructor;
            ContainsLocalAbstractCallInConstructor = visitor.ContainsLocalAbstractCallInConstructor;
            ContainsUntracedMethodCallInConstructor = visitor.ContainsUntracedMethodCallInConstructor;
            HasSubclasses = classNameToSubclasses.ContainsKey(FirstPassDetails().ClassName);
            if (HasSubclasses)
            Console.WriteLine(string.Concat(_classDef.GetText().Take(50)) + " : "
                + FirstPassDetails().SuperClassName + " : "
                + classNameToSubclasses[FirstPassDetails().ClassName].First().ClassName + " : "
                + HasSubclasses);
            _classDef = null;
        }

        public FirstPassDetails FirstPassDetails()
        {
            if (_lazyFirstPassDetails != null) return _lazyFirstPassDetails;

            var methodFinder = new MethodVisitor();
            methodFinder.VisitClass_definition(_classDef);

            _lazyFirstPassDetails = methodFinder.FirstPassDetails;
            return _lazyFirstPassDetails;
        }
    }
}
