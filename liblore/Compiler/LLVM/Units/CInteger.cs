using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileIntegerExpression (IntegerExpression expr) {
            
            // Use a 32-bit integer if the width of the value is less than or equal ti 32-bit
            LLVMTypeRef intType = LLVM.IntType (Math.Max ((uint)expr.Size, 32));

            // Compute a boolean value indicating whether the resulting
            // integer should be sign-extended.
            var signExtend = !expr.Size.HasFlag (IntegerSize.Unsigned);

            // Create a constant integer from the specified expression
            var val = LLVM.ConstInt (intType, expr.Value, signExtend);
            Stack.Push (val);
        }
    }
}

