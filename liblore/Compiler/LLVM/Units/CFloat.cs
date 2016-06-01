using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileFloatExpression (FloatExpression expr) {

            // Create a constant real
            // For the moment, doubles are used everywhere and cast
            // to float if requested.
            // That way we should not lose any information.
            var val = LLVM.ConstReal (LLVM.DoubleType (), expr.Value);
            Stack.Push (val);
        }
    }
}

