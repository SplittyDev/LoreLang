using System;
using LLVMSharp;

namespace Lore {

    /// <summary>
    /// Lore code generator.
    /// </summary>
    public partial class LoreLLVMCompiler {

        void CompileUnaryOperation (UnaryExpression expr) {

            // Compile the expression
            expr.VisitChildren (this);

            // Pop the right value from the stack
            var right = Stack.Pop ().Value;
            LLVMValueRef result = LLVMNull;

            Func<LoreException> BuildUnsupportedUnaryOperationException = () => {
                var r = right.TypeOf ().PrintTypeToString ();
                return LoreException.Create (Location)
                                       .Describe ($"Invalid unary operation: {expr.Operation}")
                                       .Describe ($"The operation is unsupported on type '{r}'");
            };

            if (Helper.IsBoolean (right)) {
                if (expr.Operation == UnaryOperation.LogicalNot) {
                } else {
                    throw BuildUnsupportedUnaryOperationException ();
                }
            } else if (Helper.IsFloatOrDouble (right)) {
                switch (expr.Operation) {
                case UnaryOperation.Negate:
                    result = LLVM.BuildFNeg (Builder, right, "unop");
                    break;
                default:
                    throw BuildUnsupportedUnaryOperationException ();
                }
            } else if (Helper.IsInteger (right)) {
                switch (expr.Operation) {
                case UnaryOperation.Negate:
                    result = LLVM.BuildNeg (Builder, right, "unop");
                    break;
                case UnaryOperation.BitwiseNot:
                    result = LLVM.BuildNot (Builder, right, "unop");
                    break;
                default:
                    throw BuildUnsupportedUnaryOperationException ();
                }
            } else {
                throw BuildUnsupportedUnaryOperationException ();
            }

            Stack.Push (Symbol.CreateAnonymous (result));
        }
    }
}

