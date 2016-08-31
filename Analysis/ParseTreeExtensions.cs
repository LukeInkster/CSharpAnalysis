
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace CSharpAnalysis
{
    public static class ParseTreeExtensions
    {
        public static IEnumerable<IParseTree> Children(this IParseTree tree)
        {
            return Enumerable
                .Range(0, tree.ChildCount)
                .Select(tree.GetChild);
        }
    }
}
