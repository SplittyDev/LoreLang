using System;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Abstract Syntax Tree Node.
    /// </summary>
    public abstract class AstNode : IVisitor {

        /// <summary>
        /// The source location of this node.
        /// </summary>
        public readonly SourceLocation Location;

        /// <summary>
        /// Initializes a new instance of the <see cref="AstNode"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        protected AstNode (SourceLocation location) {
            Location = location;
        }

        #region Implementation of the Visitor Pattern

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Visit (AstVisitor visitor);

        /// <summary>
        /// Visit the children of the specified visitor.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="visitor">Visitor.</param>
        public virtual void VisitChildren (AstVisitor visitor) { }

        #endregion
    }
}

