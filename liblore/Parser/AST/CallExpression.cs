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
        public readonly AstNode Target;

        /// <summary>
        /// The arguments of the call.
        /// </summary>
        public readonly ArgumentList Arguments;

        /// <summary>
        /// Gets whether the call has any arguments.
        /// </summary>
        /// <value>Whether the call has any arguments.</value>
        public bool HasArguments => Arguments.Count > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        CallExpression (SourceLocation location, AstNode target, ArgumentList arguments) : base (location) {
            Target = target;
            Arguments = arguments;
        }

        /// <summary>
        /// Create a new instance of the <see cref="CallExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="target">Target.</param>
        /// <param name="arguments">Arguments.</param>
        public static CallExpression Create (SourceLocation location, AstNode target, ArgumentList arguments)
        => new CallExpression (location, target, arguments);

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
    }
}

