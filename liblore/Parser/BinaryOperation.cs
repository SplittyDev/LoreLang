using System;

namespace Lore {

    /// <summary>
    /// Binary operation.
    /// </summary>
    public enum BinaryOperation {
        None,
        Assign,
        LogicalOr,
        LogicalAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseAnd,
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        ShiftLeft,
        ShiftRight,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
    }
}

