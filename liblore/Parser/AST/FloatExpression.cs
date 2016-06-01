using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Float expression node.
    /// </summary>
    public class FloatExpression : AstNode {

        /// <summary>
        /// The unsigned value.
        /// </summary>
        double value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value => value;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="fval">Fval.</param>
        FloatExpression (SourceLocation location, double fval) : base (location) {
            value = fval;
        }

        /// <summary>
        /// Create a new instance of the <see cref="FloatExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="fval">Value.</param>
        public static FloatExpression Create (SourceLocation location, double fval = 0)
        => new FloatExpression (location, fval);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="fval">Value.</param>
        public void SetValue (double fval) {
            value = fval;
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        public override string ToString () => $"[Float: Value='{value}']";
    }
}