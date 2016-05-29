using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Function.
    /// </summary>
    public class Function : AstNode {

        /// <summary>
        /// The name of the function.
        /// </summary>
        string name;

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>The name.</value>
        public string Name => name;

        /// <summary>
        /// The parameters.
        /// </summary>
        List<FunctionParameter> parameters;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public FunctionParameter [] Parameters => parameters.ToArray ();

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
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public Function (SourceLocation location) : base (location) {
            parameters = new List<FunctionParameter> ();
        }

        /// <summary>
        /// Sets the name of the function.
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="name">Name.</param>
        public void SetName (string name) {
            this.name = name;
        }

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
        public void SetParameters (List<FunctionParameter> parameters) {
            this.parameters = parameters;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="parameters">Parameters.</param>
        public void SetParameters (IEnumerable<FunctionParameter> parameters) {
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
            accum.Append ($"[Function; Name: {name}");
            if (!HasParameters) {
                accum.Append ("; Parameters: None");
            } else {
                var args = string.Join (", ", parameters);
                accum.Append ($"; Parameters: {args}");
            }
            if (!HasCaptures) {
                accum.Append ("; Captures: Nothing");
            } else {
                var captures = string.Join (", ", body.Captures.Select (c => c.ToString ()));
                accum.Append ($"; Captures: {captures}");
            }
            accum.Append ("]");
            return accum.ToString ();
        }
    }
}

