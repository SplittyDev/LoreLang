using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Abstract Syntax Tree Block.
    /// </summary>
    public sealed class AstRoot : AstNode {

        /// <summary>
        /// The children.
        /// </summary>
        readonly List<AstNode> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="AstRoot"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public AstRoot (SourceLocation location) : base (location) {
            children = new List<AstNode> ();
        }

        /// <summary>
        /// Adds a child to the node.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="node">Node.</param>
        public void AddChild (AstNode node) {
            children.Add (node);
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
            children.ForEach (child => child.Visit (visitor));
        }
    }
}

