using System;
using System.Collections;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Tuple expression node.
    /// </summary>
    public class TupleExpression : AstNode {

        /// <summary>
        /// The arguments.
        /// </summary>
        public readonly List<AstNode> Items;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => Items.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        TupleExpression (SourceLocation location) : base (location) {
            Items = new List<AstNode> ();
        }

        /// <summary>
        /// Create a new instance of the <see cref="ListExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static TupleExpression Create (SourceLocation location) => new TupleExpression (location);

        /// <summary>
        /// Add the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void Add (AstNode node) => Items.Add (node);

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
            Items.ForEach (node => node.Visit (visitor));
        }

        public override string ToString () => $"[Tuple: Count={Items.Count}]";
    }
}

