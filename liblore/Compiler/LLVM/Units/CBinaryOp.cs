using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileBinaryOperation (BinaryExpression expr) {

            // Compile the expression
            expr.VisitChildren (this);

            // Pop the right value from the stack
            var right = Stack.Pop ();
            var left = Stack.Pop ();
            LLVMValueRef result = LLVMNull;

            Func<LoreException> BuildUnsupportedBinaryOperationException = () => {
                var l = left.TypeOf ().PrintTypeToString ().Trim ();
                var r = right.TypeOf ().PrintTypeToString ().Trim ();
                return LoreException.Create (Location)
                                    .Describe ($"Invalid binary operation: {expr.Operation}")
                                    .Describe ($"The operation is unsupported on types '{l}' and '{r}'");
            };

            if (expr.Operation == BinaryOperation.Assign) {

                // Try to find the symbol
                Symbol sym;
                if (!Table.FindSymbolByRef (left, out sym)) {

                    // There is nothing to process
                    throw BuildUnsupportedBinaryOperationException ();
                }

                // Check if the variable is mutable
                if (!sym.IsMutable) {

                    // The variable is immutable
                    throw BuildUnsupportedBinaryOperationException ();
                }

                // Check if the symbol is a pointer
                if (!sym.IsPointer) {

                    // It is not possible to assign a new value to
                    // the specified variable, since it is not a pointer
                    // and is therefore not allocated on the stack.
                    // Throw an exception
                    throw LoreException.Create (Location).Describe ($"Attempt to modify constant: '{sym.Name}");
                }

                // Assign the new value to the variable
                var targetType = left.TypeOf ().GetElementType ();
                right = Helper.BuildCast (Builder, right, targetType);
                LLVM.BuildStore (Builder, right, left);

                // Set the result
                result = right;
            }

            else if (Helper.IsInteger (left) && Helper.IsInteger (right)) {
                switch (expr.Operation) {
                case BinaryOperation.Add:
                    result = LLVM.BuildAdd (Builder, left, right, "tmpadd");
                    break;
                case BinaryOperation.Subtract:
                    result = LLVM.BuildSub (Builder, left, right, "tmpsub");
                    break;
                case BinaryOperation.Divide:
                    result = LLVM.BuildUDiv (Builder, left, right, "tmpdiv");
                    break;
                case BinaryOperation.Multiply:
                    result = LLVM.BuildMul (Builder, left, right, "tmpmul");
                    break;
                default:
                    throw BuildUnsupportedBinaryOperationException ();
                }
            } else if ((Helper.IsFloatOrDouble (left) && Helper.IsInteger (right))
                       || (Helper.IsFloatOrDouble (right) && Helper.IsInteger (left))
                       || (Helper.IsFloatOrDouble (left) && Helper.IsFloatOrDouble (right))) {
                switch (expr.Operation) {
                case BinaryOperation.Add:
                    result = LLVM.BuildFAdd (Builder, left, right, "tmpfadd");
                    break;
                case BinaryOperation.Subtract:
                    result = LLVM.BuildFSub (Builder, left, right, "tmpfsub");
                    break;
                case BinaryOperation.Divide:
                    result = LLVM.BuildFDiv (Builder, left, right, "tmpfdiv");
                    break;
                case BinaryOperation.Multiply:
                    result = LLVM.BuildFMul (Builder, left, right, "tmpfmul");
                    break;
                default:
                    throw BuildUnsupportedBinaryOperationException ();
                }
            } else {
                throw BuildUnsupportedBinaryOperationException ();
            }

            Stack.Push (result);
        }
    }
}

