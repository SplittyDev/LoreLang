using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Abstract Syntax Tree Node.
    /// </summary>
    public class Dummy : AstNode {

        /// <summary>
        /// Initializes a new instance of the <see cref="Dummy"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        Dummy (SourceLocation location) : base (location) {
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }
    }
}

