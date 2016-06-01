using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileTuple (TupleExpression tuple) {
            var count = tuple.Count;
            tuple.VisitChildren (this);
        }
    }
}

