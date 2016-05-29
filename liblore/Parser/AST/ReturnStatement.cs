using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Return statement node.
    /// </summary>
    public class ReturnStatement : AstNode {

        /// <summary>
        /// The expression.
        /// </summary>
        public readonly AstNode Expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.ReturnStatement"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="expr">Value.</param>
        ReturnStatement (SourceLocation location, AstNode expr) : base (location) {
            Expression = expr;
        }

        /// <summary>
        /// Create a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="expr">Value.</param>
        public static ReturnStatement Create (SourceLocation location, AstNode expr)
        => new ReturnStatement (location, expr);

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
            Expression.Visit (visitor);
        }
    }
}

