using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexDotNet;

namespace Lore {

    /// <summary>
    /// Function declaration node.
    /// </summary>
    public class FunctionDeclaration : AstNode {

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
        List<NamedParameter> parameters;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<NamedParameter> Parameters => parameters;

        /// <summary>
        /// The return type.
        /// </summary>
        NameExpression returnType;

        /// <summary>
        /// Gets the return type.
        /// </summary>
        /// <value>The return type.</value>
        public NameExpression ReturnType => returnType;

        /// <summary>
        /// The tuple return types.
        /// </summary>
        List<NameExpression> tupleReturnTypes;

        /// <summary>
        /// Gets the tuple return types.
        /// </summary>
        /// <value>The tuple return types.</value>
        public List<NameExpression> TupleReturnTypes => tupleReturnTypes;

        /// <summary>
        /// Gets whether the function has a return type.
        /// </summary>
        /// <value>Whether the function has a return type.</value>
        public bool HasReturnType => returnType != null;

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
        /// Gets whether the function returns a tuple.
        /// </summary>
        /// <value>Whether the function returns a tuple.</value>
        public bool ReturnsTuple => tupleReturnTypes.Count > 0;

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
        /// Initializes a new instance of the <see cref="FunctionDeclaration"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        FunctionDeclaration (SourceLocation location) : base (location) {
            parameters = new List<NamedParameter> ();
            tupleReturnTypes = new List<NameExpression> ();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FunctionDeclaration"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        public static FunctionDeclaration Create (SourceLocation location) => new FunctionDeclaration (location);

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
        /// Sets the return type.
        /// </summary>
        /// <returns>The return type.</returns>
        /// <param name="expr">Expr.</param>
        public void SetReturnType (NameExpression expr) {
            returnType = expr;
        }

        /// <summary>
        /// Sets the return tuple.
        /// </summary>
        /// <returns>The return tuple.</returns>
        /// <param name="exprs">Exprs.</param>
        public void SetReturnTuple (IEnumerable<NameExpression> exprs) {
            tupleReturnTypes.AddRange (exprs);
        }

        /// <summary>
        /// Visit the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Visit (AstVisitor visitor) {
            visitor.Accept (this);
        }

        public override void VisitChildren (AstVisitor visitor) {
            body.VisitChildren (visitor);
        }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString () {
            var accum = new StringBuilder ();
            accum.Append ($"[Function: Name='{name}'");
            if (HasParameters) {
                var args = string.Join (", ", parameters);
                accum.Append ($" Args=[{args}]");
            }
            if (HasCaptures) {
                var captures = string.Join (", ", body.Captures.Select (c => c.ToString ()));
                accum.Append ($" Captures=[{captures}]"); 
            }
            if (HasReturnType) {
                accum.Append ($" Returns='{returnType.Name}'");
            }
            accum.Append ("]");
            return accum.ToString ();
        }
    }
}

