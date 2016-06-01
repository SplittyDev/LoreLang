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
        AstNode expression;

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public AstNode Expression => expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lore.ReturnStatement"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        ReturnStatement (SourceLocation location) : base (location) {
        }

        /// <summary>
        /// Create a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static ReturnStatement Create (SourceLocation location)
        => new ReturnStatement (location);

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <returns>The expression.</returns>
        /// <param name="expr">Expr.</param>
        public void SetExpression (AstNode expr) {
            expression = expr;
        }

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

        public override string ToString () => $"[Return: Value={Expression.ToString () ?? string.Empty}]";
    }
}

