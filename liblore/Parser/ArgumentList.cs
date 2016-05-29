using System;
using System.Collections;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// List node.
    /// </summary>
    public class ArgumentList : AstNode {

        /// <summary>
        /// The arguments.
        /// </summary>
        public readonly List<AstNode> Arguments;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => Arguments.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentList"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        ArgumentList (SourceLocation location) : base (location) {
            Arguments = new List<AstNode> ();
        }

        /// <summary>
        /// Create a new instance of the <see cref="ArgumentList"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static ArgumentList Create (SourceLocation location) => new ArgumentList (location);

        /// <summary>
        /// Add the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void Add (AstNode node) => Arguments.Add (node);

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
            Arguments.ForEach (node => node.Visit (visitor));
        }
    }
}

