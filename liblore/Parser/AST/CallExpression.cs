using System;
using System.Collections.Generic;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Call expression node.
    /// </summary>
    public class CallExpression : AstNode {

        /// <summary>
        /// The target of the call.
        /// </summary>
        AstNode target;

        /// <summary>
        /// The arguments of the call.
        /// </summary>
        ArgumentList arguments;

        /// <summary>
        /// Gets whether the call has any arguments.
        /// </summary>
        /// <value>Whether the call has any arguments.</value>
        public bool HasArguments => arguments.Count > 0;

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public AstNode Target => target;

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public ArgumentList Arguments => arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        CallExpression (SourceLocation location) : base (location) {
        }

        /// <summary>
        /// Create a new instance of the <see cref="CallExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static CallExpression Create (SourceLocation location)
        => new CallExpression (location);

        /// <summary>
        /// Sets the target.
        /// </summary>
        /// <returns>The target.</returns>
        /// <param name="target">Target.</param>
        public void SetTarget (AstNode target) {
            this.target = target;
        }

        /// <summary>
        /// Sets the arguments.
        /// </summary>
        /// <returns>The arguments.</returns>
        /// <param name="args">Arguments.</param>
        public void SetArguments (ArgumentList args) {
            arguments = args;
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
            Target.Visit (visitor);
            Arguments.Visit (visitor);
        }

        public override string ToString () => $"[Call]";
    }
}

