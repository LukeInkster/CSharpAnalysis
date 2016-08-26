using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAnalysis
{
    public class ClassAnalysis
    {
        public int ExtendingClassCount { get; }
        public int ClassCount { get; }
        public int MethodCount { get; }
        public int VirtualMethodCount { get; }
        public int OverrideMethodCount { get; }

        public ClassAnalysis(CSharpParser.Class_definitionContext classDef)
        {
            // Visitors need to stay local scoped. It appears they keep a reference to the
            // trees they visit, so if we keep a reference to the visitor then we also
            // keep a transitive reference to the tree which prevents garbage collection
            var visitor = new CSharpClassVisitor();

            visitor.VisitClass_definition(classDef);

            ClassCount = visitor.ClassCount;
            ExtendingClassCount = visitor.ExtendingClassCount;
            MethodCount = visitor.MethodCount;
            VirtualMethodCount = visitor.VirtualMethodCount;
            OverrideMethodCount = visitor.OverrideMethodCount;
        }
    }
}
