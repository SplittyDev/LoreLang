using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileName (NameExpression expr) {

            // Check if the variable exists
            Symbol sym;
            if (!Table.FindSymbol (expr.Name, out sym)) {

                // The variable does not exist
                // Throw an exception
                throw LoreException.Create (Location).Describe ($"Undefined variable: '{expr.Name}'");
            }

            // TODO: Check if the variable was captured

            // Push the variable
            Stack.Push (sym.Value);
        }
    }
}

