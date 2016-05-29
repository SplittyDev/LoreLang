using System;
using System.Collections.Generic;
using System.Linq;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Abstract Syntax Tree Block.
    /// </summary>
    public sealed class CodeBlock : AstNode {

        /// <summary>
        /// The captures.
        /// </summary>
        public readonly List<Capture> Captures;

        /// <summary>
        /// The children.
        /// </summary>
        readonly List<AstNode> Children;

        /// <summary>
        /// Gets whether the code block captures anything.
        /// </summary>
        /// <value>Whether the code block captures anything.</value>
        public bool HasCaptures => Captures.Count > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlock"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public CodeBlock (SourceLocation location) : base (location) {
            Captures = new List<Capture> ();
            Children = new List<AstNode> ();
        }

        /// <summary>
        /// Adds a child to the node.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="node">Node.</param>
        public void AddChild (AstNode node) {
            Children.Add (node);
        }

        /// <summary>
        /// Adds a capture to the node.
        /// </summary>
        /// <returns>The capture.</returns>
        /// <param name="capture">Capture.</param>
        public void AddCapture (Capture capture) {
            Captures.Add (capture);
        }

        /// <summary>
        /// Merge this code block with another one.
        /// </summary>
        /// <param name="other">The other code block.</param>
        public void Merge (CodeBlock other) {
            Captures.AddRange (other.Captures.GroupBy (c => c.Identifier).Select (g => g.First ()));
            Children.AddRange (other.Children);
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
            Children.ForEach (child => child.Visit (visitor));
        }
    }
}

