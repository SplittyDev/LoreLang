using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileString (StringExpression expr) {
            var strlen = (uint) expr.Value.Length;
            var arr = LLVM.ArrayType (LLVM.Int8Type (), strlen + 1);
            var val = LLVM.AddGlobal (LLVMModule, arr, "string");
            LLVM.SetLinkage (val, LLVMLinkage.LLVMInternalLinkage);
            LLVM.SetGlobalConstant (val, true);
            var str = LLVM.ConstString (expr.Value, strlen, false);
            LLVM.SetInitializer (val, str);
            Stack.Push (Symbol.CreateAnonymous (val, SpecialValue.String));
        }
    }
}

