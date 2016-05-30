using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Integer expression node.
    /// </summary>
    public class IntegerExpression : AstNode {

        /// <summary>
        /// The unsigned value.
        /// </summary>
        ulong unsignedValue;

        /// <summary>
        /// The size.
        /// </summary>
        IntegerSize size;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public ulong Value => unsignedValue;

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public IntegerSize Size => size;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        IntegerExpression (SourceLocation location, ulong unsigned) : base (location) {
            unsignedValue = unsigned;
        }

        /// <summary>
        /// Create a new instance of the <see cref="IntegerExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="unsigned">Unsigned.</param>
        public static IntegerExpression Create (SourceLocation location, ulong unsigned = 0)
        => new IntegerExpression (location, unsigned);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="unsigned">Unsigned.</param>
        public void SetValue (ulong unsigned) {
            unsignedValue = unsigned;
        }

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <returns>The size.</returns>
        /// <param name="size">Size.</param>
        public void SetSize (IntegerSize size) {
            this.size = size;
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        public override string ToString () => $"[Integer: Value='{unsignedValue}' Size='{size}']";
    }
}