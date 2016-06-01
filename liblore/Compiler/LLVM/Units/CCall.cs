using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileCall (CallExpression call) {

            // Try to resolve the function
            Symbol sym;
            var callNameExpr = call.Target as NameExpression;
            if (callNameExpr != null) {

                // Check if the target function is in scope
                if (!Table.FindSymbol (callNameExpr.Name, out sym)) {

                    // The function is undefined or not in scope
                    // Throw an exception
                    throw LoreException.Create (Location).Describe ($"Attempt to call undefined function '{callNameExpr.Name}'");
                }

                // Parse the arguments
                var args = new LLVMValueRef [Math.Max (call.Arguments.Count, 0)];
                for (var i = 0; i < args.Length; i++) {
                    call.Arguments.Arguments [i].Visit (this);
                    args [i] = Stack.Pop ();
                }

                // The function is in scope
                var callResult = LLVM.BuildCall (Builder, sym.Value, args, "tmpcall");
                Stack.Push (callResult);
            } else {
                throw LoreException.Create (Location).Describe ($"Attempt to call something that is everything but a function.");
            }
        }
    }
}

