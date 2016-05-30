using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Name expression node.
    /// </summary>
    public class NameExpression : AstNode {

        /// <summary>
        /// The name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        NameExpression (SourceLocation location, string name) : base (location) {
            Name = name;
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        /// <summary>
        /// Create a new instance of the <see cref="NameExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="name">Name.</param>
        public static NameExpression Create (SourceLocation location, string name) => new NameExpression (location, name);

        public override string ToString () => $"[Name: Value={Name}]";
    }
}

