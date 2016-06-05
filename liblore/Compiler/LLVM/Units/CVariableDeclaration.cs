using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileAssignment (AssignStatement assign) {
            for (var i = 0; i < assign.IdentifierCount; i++) {
                CompileAssignment (assign, assign.Identifiers [i], assign.Expressions [i]);
            }
        }

        void CompileAssignment (AssignStatement stmt, NamedParameter identifier, AstNode expr) {

            // Check if the variable already exists in the parent scope
            Symbol sym;
            if (Table.TopScope.FindSymbol (identifier.Name.Name, out sym)) {

                // The variable already exists
                throw LoreException.Create (Location)
                                   .Describe ($"Redefinition of variable '{identifier.Name.Name}'.")
                                   .Describe ($"Previous definition was at '{sym.Location}'.");
            }

            // Compile the expression
            expr.Visit (this);

            // Get the expression value and type
            var exprVal = Stack.Pop ().Value;
            var exprValType = exprVal.TypeOf ();

            // Check if the identifier has a type
            if (identifier.HasType) {
                var identType = Helper.GetBuiltinTypeFromString (identifier.Type.Name);

                // Check if the type of the identifier is different
                // from the type of the expression
                if (!Helper.CompareType (identType, exprValType)) {

                    // Try casting the expression value to the expected type
                    exprVal = Helper.BuildCast (Builder, exprVal, identType);
                }

                // Set the type of the expression to the type of the identifier
                exprValType = identType;
            }

            // Allocate the variable
            var ptr = LLVM.BuildAlloca (Builder, exprValType, identifier.Name.Name);

            // Store the value in the variable
            LLVM.BuildStore (Builder, exprVal, ptr);

            // Add the variable to the symbol table
            var name = identifier.Name.Name;
            sym = Symbol.CreatePtr (name, ptr, Location, stmt.Mutable);
            Table.AddSymbol (sym);
        }
    }
}