using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Lambda expression node.
    /// </summary>
    public class LambdaExpression : AstNode {

        /// <summary>
        /// The parameters.
        /// </summary>
        List<NamedParameter> parameters;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public NamedParameter [] Parameters => parameters.ToArray ();

        /// <summary>
        /// Gets whether the function has parameters.
        /// </summary>
        /// <value>Whether the function has parameters.</value>
        public bool HasParameters => parameters.Any ();

        /// <summary>
        /// Gets whether the function body captures anything.
        /// </summary>
        /// <value>Whether the function body captures anything.</value>
        public bool HasCaptures => body.HasCaptures;

        /// <summary>
        /// The body of the function.
        /// </summary>
        CodeBlock body;

        /// <summary>
        /// Gets the body of the function.
        /// </summary>
        /// <value>The body.</value>
        public CodeBlock Body => body;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        LambdaExpression (SourceLocation location) : base (location) {
            parameters = new List<NamedParameter> ();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LambdaExpression"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static LambdaExpression Create (SourceLocation location) => new LambdaExpression (location);

        /// <summary>
        /// Sets the body of the function.
        /// </summary>
        /// <returns>The body.</returns>
        /// <param name="body">Body.</param>
        public void SetBody (CodeBlock body) {
            this.body = body;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="parameters">Parameters.</param>
        public void SetParameters (List<NamedParameter> parameters) {
            this.parameters = parameters;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="parameters">Parameters.</param>
        public void SetParameters (IEnumerable<NamedParameter> parameters) {
            this.parameters = parameters.ToList ();
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString () {
            var accum = new StringBuilder ();
            accum.Append ($"[Lambda:");
            if (HasParameters) {
                var args = string.Join (", ", parameters);
                accum.Append ($" Args=[{args}]");
            }
            if (HasCaptures) {
                var captures = string.Join (", ", body.Captures.Select (c => c.ToString ()));
                accum.Append ($" Captures=[{captures}]");
            }
            accum.Append ("]");
            return accum.ToString ();
        }
    }
}

