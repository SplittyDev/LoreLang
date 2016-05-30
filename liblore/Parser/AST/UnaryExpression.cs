using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Unary expression node.
    /// </summary>
    public class UnaryExpression : AstNode {

        /// <summary>
        /// The operation.
        /// </summary>
        public readonly UnaryOperation Operation;

        /// <summary>
        /// The child node.
        /// </summary>
        public readonly AstNode Child;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.UnaryExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="op">Op.</param>
        /// <param name="child">Child.</param>
        UnaryExpression (SourceLocation location, UnaryOperation op, AstNode child) : base (location) {
            Operation = op;
            Child = child;
        }

        /// <summary>
        /// Create a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="op">Op.</param>
        /// <param name="child">Right.</param>
        public static UnaryExpression Create (SourceLocation location, UnaryOperation op, AstNode child)
        => new UnaryExpression (location, op, child);

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        /// <summary>
        /// Visits the children.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="visitor">Visitor.</param>
        public override void VisitChildren (AstVisitor visitor) {
            Child.Visit (visitor);
        }

        public override string ToString () => $"[Unary: Op={Operation}]";
    }
}

